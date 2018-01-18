FROM microsoft/dotnet:runtime
COPY /deploy /

COPY sshd_config /etc/ssh/
COPY init_container.sh /bin/


RUN apt-get update \
    && apt-get install -y --no-install-recommends openssh-server \
    && echo "root:Docker!" | chpasswd

RUN chmod 755 /bin/init_container.sh

CMD ["/bin/init_container.sh"]

WORKDIR /Server

EXPOSE 2222 8085

ENTRYPOINT ["dotnet", "Server.dll"]