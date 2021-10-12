FROM node:14.18.0-alpine

ARG API_URL
ARG ENABLE_DEMO_MODE

ENV REACT_APP_API_URL ${API_URL}
ENV REACT_APP_IS_DEMO ${ENABLE_DEMO_MODE}

WORKDIR /app

COPY . .

COPY package.json /app/package.json
COPY package-lock.json /app/package-lock.json

RUN npm install

EXPOSE 3000

CMD [ "npm", "start" ]
