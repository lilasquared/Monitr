# Monitr

Get started with docker-compose

```yml
version: "3"

services:
  app:
    image: lilasquared/monitr:1.0
    ports:
      - "8080:80"
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
```

`$ docker-compose up`
