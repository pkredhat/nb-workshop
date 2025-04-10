import os
import sys
import asyncio
import logging
import argparse
from aiokafka import AIOKafkaProducer

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

async def produce(topic: str):
    logger.info("Starting Kafka Producer...")
    bootstrap_servers = os.environ.get("KAFKA_BOOTSTRAP_SERVERS")

    if not bootstrap_servers:
        logger.error("KAFKA_BOOTSTRAP_SERVERS environment variable is not set.")
        return

    logger.info(f"KAFKA_BOOTSTRAP_SERVERS: {bootstrap_servers}")
    logger.info(f"Kafka topic: {topic}")

    producer = AIOKafkaProducer(bootstrap_servers=bootstrap_servers)

    try:
        await producer.start()
    except Exception as e:
        logger.error(f"Failed to start Kafka producer: {e}")
        return

    try:
        logger.info("Type your message and press Enter (type 'exit' to quit):")
        loop = asyncio.get_running_loop()
        while True:
            print("> ", end="", flush=True)
            message = await loop.run_in_executor(None, sys.stdin.readline)
            message = message.strip()
            
            if not message:
                continue
            
            if message.lower() == "exit":
                break

            try:
                result = await producer.send_and_wait(topic, message.encode('utf-8'))
                logger.info(f"✅ Delivered message to {result.topic}-{result.partition}@{result.offset}")
            except Exception as e:
                logger.error(f"❌ Delivery failed: {e}")

    finally:
        await producer.stop()
        logger.info("Kafka producer stopped.")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Kafka Producer")
    parser.add_argument(
        "--topic",
        type=str,
        default="test-topic",
        help="Kafka topic to produce messages to (default: test-topic)",
    )
    args = parser.parse_args()
    asyncio.run(produce(args.topic))
