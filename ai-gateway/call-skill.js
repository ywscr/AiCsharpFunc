/**
 * call-skill.js
 *
 * HTTP client for the C# Skill Service.
 * Builds the required service-to-service authentication headers:
 *   X-Client-Id, X-Api-Key, X-Timestamp, X-Nonce, X-Signature
 *
 * Signature algorithm: HMAC-SHA256( key=SKILL_HMAC_SECRET, msg="{timestamp}\n{nonce}\n{body}" )
 * Encoding: lowercase hex
 */

import { createHmac, randomUUID } from 'node:crypto';

const SKILL_URL     = process.env.SKILL_URL            || 'http://localhost:5000';
const CLIENT_ID     = process.env.SKILL_CLIENT_ID      || 'ai-gateway';
const API_KEY       = process.env.SKILL_API_KEY        || 'gw-api-key-change-me';
const HMAC_SECRET   = process.env.SKILL_HMAC_SECRET    || 'gw-hmac-secret-change-me';

/**
 * Build the authentication headers for a given request body.
 * @param {string} body - Raw JSON string of the request body.
 * @returns {Record<string, string>} Headers to attach to the request.
 */
function buildAuthHeaders(body) {
  const timestamp = String(Math.floor(Date.now() / 1000));
  const nonce     = randomUUID();
  const message   = `${timestamp}\n${nonce}\n${body}`;
  const signature = createHmac('sha256', HMAC_SECRET)
    .update(message)
    .digest('hex');

  return {
    'Content-Type': 'application/json',
    'X-Client-Id':  CLIENT_ID,
    'X-Api-Key':    API_KEY,
    'X-Timestamp':  timestamp,
    'X-Nonce':      nonce,
    'X-Signature':  signature,
  };
}

/**
 * Call POST /api/skill/faq-match on the C# Skill Service.
 * @param {string} question - The user question to match.
 * @returns {Promise<{matched: boolean, answer: string, category: string, faqId: string}>}
 */
export async function callFaqMatch(question) {
  const body    = JSON.stringify({ question });
  const headers = buildAuthHeaders(body);

  const resp = await fetch(`${SKILL_URL}/api/skill/faq-match`, {
    method: 'POST',
    headers,
    body,
  });

  if (!resp.ok) {
    const errText = await resp.text();
    throw new Error(`Skill service returned ${resp.status}: ${errText}`);
  }

  return /** @type {any} */ (await resp.json());
}
