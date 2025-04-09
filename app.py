# app.py
import sys
import asyncio
import producer
import consumer
from dotenv import load_dotenv

load_dotenv()

async def main():
    if len(sys.argv) < 2:
        print("Usage: python app.py <produce|consume>")
        return

    command = sys.argv[1].lower()

    if command == "produce":
        # Call produce.py's async produce function
        await producer.produce()

    elif command == "consume":
        # Call the async consumer function
        await consumer.consume()

    else:
        print("Invalid argument. Use 'produce' or 'consume'.")

if __name__ == "__main__":
    asyncio.run(main())
