# logstream
Client-server system for ingesting, indexing and querying structured log and event data
## Features

- **Log Ingestion:** Efficiently receive and process logs from multiple sources.
- **Indexing:** Store logs in a structured format for fast retrieval.
- **Querying:** Powerful query interface to search and filter log/event data.
- **Client-Server Architecture:** Scalable design with clear separation between client and server components.
- **Extensible:** Easily add new log sources or query capabilities.

## Getting Started

### Prerequisites

- Python 3.8+
- pip

### Installation

Clone the repository:

```bash
git clone https://github.com/oliviavasquez/logstream.git
cd logstream
```

Install dependencies:

```bash
pip install -r requirements.txt
```

### Running the Server

Start the logstream server:

```bash
python logstream/server.py
```

The server will listen for incoming log data and provide a query API.

### Sending Logs

Use the client to send logs:

```bash
python logstream/client.py --source /path/to/logfile.log
```

### Querying Logs

Query logs via the API or CLI:

```bash
python logstream/query.py --filter "level=ERROR"
```

## Configuration

Configuration options are available in `config.yaml`. You can set:

- Log sources
- Indexing strategies
- Query parameters
- Server port

## Example Usage

1. Start the server.
2. Send logs from multiple clients.
3. Query logs for specific events, errors, or patterns.

## Project Structure

```
logstream/
├── client.py
├── server.py
├── query.py
├── indexer.py
├── config.yaml
└── README.md
```

## Contributing

Contributions are welcome! Please open issues or submit pull requests for bug fixes, features, or documentation improvements.

## License

This project is licensed under the MIT License.

## Contact

For questions or support, please contact [Olivia Vasquez](mailto:olivia@fakemail.com).