/**
 * openai-flow.js
 *
 * Orchestrates the AI Gateway flow using the OpenAI Responses API:
 *
 *   1. Send the user question to OpenAI with the `faq_match` function tool registered.
 *   2. If the model issues a function call → execute the C# Skill Service.
 *      a. If the skill matched  → return the deterministic answer directly (no LLM re-write).
 *      b. If the skill did not match → send the tool output back to OpenAI and return its reply.
 *   3. If the model answers directly (no tool call) → return that answer.
 */

import OpenAI from 'openai';
import { callFaqMatch } from './call-skill.js';

const openai = new OpenAI({ apiKey: process.env.OPENAI_API_KEY });

/** Tool definition for the FAQ match function */
const FAQ_TOOL = {
  type: 'function',
  name: 'faq_match',
  description:
    'Query the enterprise FAQ knowledge base. ' +
    'MUST be called for any question about products, policies, refunds, shipping, invoices, or support ' +
    'before generating an answer from general knowledge.',
  parameters: {
    type: 'object',
    properties: {
      question: {
        type: 'string',
        description: 'The user question to look up in the FAQ database.',
      },
    },
    required: ['question'],
  },
};

/**
 * Run the full question-answering flow.
 *
 * @param {string} userMessage
 * @returns {Promise<{source: string, answer: string, faqId?: string, category?: string}>}
 */
export async function runFlow(userMessage) {
  const model = process.env.OPENAI_MODEL || 'gpt-4o-mini';

  // ── Step 1: Ask OpenAI — it may call our FAQ skill ──────────────────────────
  const response = await openai.responses.create({
    model,
    input: userMessage,
    tools: [FAQ_TOOL],
  });

  // ── Step 2: Did the model request a tool call? ───────────────────────────────
  const toolCall = response.output.find(
    (item) => item.type === 'function_call' && item.name === 'faq_match',
  );

  if (!toolCall) {
    // Model answered without calling a tool
    return {
      source: 'model',
      answer: response.output_text ?? '',
    };
  }

  // ── Step 3: Execute the C# Skill Service ─────────────────────────────────────
  const args = JSON.parse(toolCall.arguments);
  let skillResult;
  try {
    skillResult = await callFaqMatch(args.question);
  } catch (err) {
    skillResult = { matched: false, error: String(err.message) };
  }

  // ── Step 4a: Skill hit → return the deterministic answer directly ────────────
  if (skillResult.matched) {
    return {
      source:   'skill',
      faqId:    skillResult.faqId,
      category: skillResult.category,
      answer:   skillResult.answer,
    };
  }

  // ── Step 4b: Skill missed → feed output back to OpenAI for a natural reply ───
  const response2 = await openai.responses.create({
    model,
    previous_response_id: response.id,
    input: [
      {
        type:    'function_call_output',
        call_id: toolCall.call_id,
        output:  JSON.stringify(skillResult),
      },
    ],
  });

  return {
    source: 'model_with_tool',
    answer: response2.output_text ?? '',
  };
}
