FROM postgres:14

COPY ./migrations/*.sql /docker-entrypoint-initdb.d