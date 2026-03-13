/**
 * server.js
 *
 * AI Gateway HTTP server (Express).
 * Exposes a single endpoint:
 *
 *   POST /ask   { "question": "..." }
 *   GET  /health
 */

import 'dotenv/config';
import express from 'express';
import { runFlow } from './openai-flow.js';

const app = express();
app.use(express.json());

/**
 * POST /ask
 * Body: { "question": "<user question>" }
 * Response: { "source": "skill|model|model_with_tool", "answer": "...", ... }
 */
app.post('/ask', async (req, res) => {
  const { question } = req.body ?? {};

  if (!question || typeof question !== 'string' || !question.trim()) {
    return res.status(400).json({ error: 'question is required and must be a non-empty string.' });
  }

  try {
    const result = await runFlow(question.trim());
    return res.json(result);
  } catch (err) {
    console.error('[gateway] flow error:', err);
    return res.status(500).json({ error: 'Internal error', detail: err.message });
  }
});

/** GET /health — liveness probe */
app.get('/health', (_req, res) => res.json({ status: 'ok' }));

const PORT = Number(process.env.PORT) || 3000;
app.listen(PORT, () => {
  console.log(`[gateway] listening on http://localhost:${PORT}`);
  console.log(`[gateway] skill service → ${process.env.SKILL_URL || 'http://localhost:5000'}`);
});
