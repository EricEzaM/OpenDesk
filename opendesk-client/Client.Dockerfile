# Build stage, npm install and build
FROM node:14-alpine as build

ARG API_URL
ARG ENABLE_DEMO_MODE

ENV REACT_APP_API_URL ${API_URL}
ENV REACT_APP_IS_DEMO ${ENABLE_DEMO_MODE}

WORKDIR /app

COPY . .

COPY package.json /app/package.json
COPY package-lock.json /app/package-lock.json

RUN npm install
RUN npm run build

# Final stage, run Nginx server.
FROM nginx:alpine as final

COPY --from=build /app/build /usr/share/nginx/html
COPY ./nginx/*.conf /etc/nginx/conf.d
