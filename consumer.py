import os
import uuid
import argparse
import asyncio
from aiokafka import AIOKafkaConsumer

async def consume(topic: str):
    bootstrap_servers = os.environ.get("KAFKA_BOOTSTRAP_SERVERS", "localhost:9092")
    group_id = f"test-group-{uuid.uuid4()}"

    if not bootstrap_servers:
        print("KAFKA_BOOTSTRAP_SERVERS environment variable is not set.")
        return

    print(f"KAFKA_BOOTSTRAP_SERVERS: {bootstrap_servers}")
    print(f"Kafka topic: {topic}")

    consumer = AIOKafkaConsumer(
        topic,
        loop=asyncio.get_event_loop(),
        bootstrap_servers=bootstrap_servers,
        group_id=group_id,
        auto_offset_reset='earliest'
    )

    await consumer.start()
    try:
        print(f"Subscribed to {topic}, waiting for messages (async)...")
        async for msg in consumer:
            print(f"📩 Received: {msg.value.decode('utf-8')}")
    finally:
        await consumer.stop()
        print("Kafka consumer stopped.")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Kafka Consumer")
    parser.add_argument(
        "--topic",
        type=str,
        default="test-topic",
        help="Kafka topic to consume messages from (default: test-topic)"
    )
    args = parser.parse_args()

    try:
        asyncio.run(consume(args.topic))
    except KeyboardInterrupt:
        print("\nStopped by user.")
