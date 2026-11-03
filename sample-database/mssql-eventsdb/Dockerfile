FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

ENV DEBIAN_FRONTEND=noninteractive
ENV PATH="/opt/mssql-tools18/bin:${PATH}"

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl \
        gnupg \
    && curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | tee /etc/apt/trusted.gpg.d/microsoft.asc >/dev/null \
    && curl -fsSL https://packages.microsoft.com/config/ubuntu/22.04/mssql-server-2022.list -o /etc/apt/sources.list.d/mssql-server-2022.list \
    && curl -fsSL https://packages.microsoft.com/config/ubuntu/22.04/prod.list -o /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y --no-install-recommends \
        locales \
        mssql-server-fts \
        mssql-tools18 \
        tzdata \
    && locale-gen hr_HR.UTF-8 \
    && update-locale LANG=hr_HR.UTF-8 LC_ALL=hr_HR.UTF-8 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

COPY scripts/entrypoint.sh /usr/local/bin/entrypoint.sh
COPY scripts/configure-db.sh /usr/local/bin/configure-db.sh

RUN chmod +x /usr/local/bin/entrypoint.sh /usr/local/bin/configure-db.sh

USER mssql

ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]
