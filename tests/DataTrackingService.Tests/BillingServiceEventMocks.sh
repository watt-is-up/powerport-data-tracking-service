docker exec -it kafka bash

kafka-console-producer \
  --bootstrap-server kafka:9092 \
  --topic billing.events <<'EOF'
{"EventId":"7h2b0b1e-9d4c-4a6e-8a0a-6a9a2c1b4e01","EventType":"ChargingSessionFinalized","EventVersion":1,"OccurredAt":"2026-01-09T10:00:00Z","Producer":"billing-service","Key":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","Payload":{"Id":0,"SessionId":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","ProviderId":"f9d2k3d7-8b3e-4f2d-9c6a-2f8b1a7d4e01","UserId":"c9d4e1a7-8b3e-4f2d-9c6a-2f8b1a7d4e01","Amount":20.2,"SessionStarted":"2026-01-09T10:00:00Z","SessionEnded":"2026-01-09T11:00:00Z","TotalEnergyKwh":4.75,"Rate":3.4}}
{"EventId":"9f8d6c3e-4f1b-4c9a-9e2d-7b8c1f2a3d02","EventType":"ChargingSessionFinalized","EventVersion":1,"OccurredAt":"2026-01-09T10:15:00Z","Producer":"billing-service","Key":"9d0d3d44-7f42-4a3a-8e67-0c2a4c2f1a01","Payload":{"Id":1,"SessionId":"9d0d3d44-7f42-4a3a-8e67-0c2a4c2f1a01","ProviderId":"g7e2k3d1-5e7a-4c9d-8a31-1c7f2d9b8a01","UserId":"b7e6c9d1-5e7a-4c9d-8a31-1c7f2d9b8a01","Amount":33.2,"SessionStarted":"2026-01-09T10:00:00Z","SessionEnded":"2026-01-09T11:00:00Z","TotalEnergyKwh":6.75,"Rate":4.4}}
EOF


kafka-consumer-groups --bootstrap-server kafka:9092 --describe --group billing-service
kafka-console-consumer \
  --bootstrap-server kafka:9092 \
  --topic charging-session.events \
  --from-beginning


kafka-console-producer \
  --bootstrap-server kafka:9092 \
  --topic billing.events <<'EOF'
{"EventId":"7h2b0b1e-9d4c-4a6e-8a0a-6a9a2c1b4e01","EventType":"ChargingSessionFinalized","EventVersion":1,"OccurredAt":"2026-01-09T10:00:00Z","Producer":"billing-service","Key":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","Payload":{"Id":0,"SessionId":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","ProviderId":"f9d2k3d7-8b3e-4f2d-9c6a-2f8b1a7d4e01","UserId":"c9d4e1a7-8b3e-4f2d-9c6a-2f8b1a7d4e01","Amount":20.2,"SessionStarted":"2026-01-09T10:00:00Z","SessionEnded":"2026-01-09T11:00:00Z","TotalEnergyKwh":4.75,"Rate":3.4}}
EOF
