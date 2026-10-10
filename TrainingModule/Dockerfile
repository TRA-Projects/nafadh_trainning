# Build context is the JobseekerModule folder:
#   docker build --build-arg API_BASE_URL=https://your-api.up.railway.app/ .
# On Railway: set the service root directory to "JobseekerModule" and define
# a service variable API_BASE_URL so it is injected as a build arg.

FROM node:22-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .

# Point the production build at the deployed API.
# Railway injects service vars as env (and as matching --build-arg when declared).
# runtime-config.json wins over environment.apiBaseUrl at runtime — patch both.
ARG API_BASE_URL
ENV API_BASE_URL=$API_BASE_URL
RUN if [ -n "$API_BASE_URL" ]; then \
      sed -i "s|https://your-api-host.example.com/|$API_BASE_URL|" src/environments/environment.prod.ts; \
      sed -i "s|http://localhost:58888/|$API_BASE_URL|" public/runtime-config.json; \
    fi
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/jobseeker-client/browser /usr/share/nginx/html
# nginx:alpine runs envsubst on /etc/nginx/templates/*.template at startup (PORT substitution)
COPY nginx.conf.template /etc/nginx/templates/default.conf.template
ENV PORT=8080
EXPOSE 8080
