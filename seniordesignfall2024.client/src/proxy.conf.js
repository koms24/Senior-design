const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7268';

const PROXY_CONFIG = [
  {
    context: [
      "/Stream"
    ],
    target,
    secure: false
  },

  {
    context: [
      "/api"
    ],
    target,
    secure: false
  }
]

module.exports = PROXY_CONFIG;
