FROM node:20-alpine
WORKDIR /app

# Install dependencies first (layer caching)
COPY package*.json ./
RUN npm ci --omit=dev

# Copy source
COPY server.js openai-flow.js call-skill.js ./

EXPOSE 3000
ENV NODE_ENV=production
CMD ["node", "server.js"]
