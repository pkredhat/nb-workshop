# Dockerfile
FROM registry.access.redhat.com/ubi8/ubi-minimal

# Install bash and curl
USER root
RUN microdnf install bash curl && \
    microdnf clean all

# Copy the script into the container at /opt
COPY apicheck.sh /opt/apicheck.sh
RUN chmod +x /opt/apicheck.sh

# Set the default command to run the script
CMD ["/opt/apicheck.sh"]
