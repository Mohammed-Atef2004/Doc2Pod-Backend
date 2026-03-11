# Doc2Bod - Arabic Podcast Generator (Backend)

**Doc2Bod** is a scalable backend service that transforms PDF documents into high-quality **Egyptian Arabic podcasts** using AI.  

Upload any PDF → Extract content → Convert to natural Egyptian Arabic → Generate professional audio podcast.

---

## ✨ Features

- **PDF Upload via REST API** – Simple drag-and-drop or file upload endpoint
- **Intelligent Content Extraction** – Handles tables, images captions, Arabic/English mixed text
- **Egyptian Arabic Adaptation** – AI-powered rewriting in authentic Egyptian dialect
- **AI-Powered Podcast Generation** – Text-to-Speech with natural Egyptian voices
- **Service-Based Architecture** – Microservices-ready, easily scalable
- **Extensible Design** – Plug in new AI models (OpenAI, Grok, ElevenLabs, Azure, etc.) with minimal effort
- **Async Processing** – Background jobs for large documents (Celery/RabbitMQ ready)
- **Audio Download & Streaming** – Returns direct MP3 URL or file

---

## 📁 Project Structure

```bash
Doc2Bod-Backend/
├── src/
│   ├── controllers/    # FastAPI/Express/ASP.NET controllers & API endpoints
│   ├── services/       # PDF parser, text processor, TTS engine, Egyptian dialect adapter
│   ├── models/         # Pydantic/SQLAlchemy data models & AI response schemas
│   ├── utils/          # Helpers, logging, file handling, validation
│   └── core/           # Config, database, Redis, Celery setup
├── tests/              # Unit + integration tests (pytest or Jest)
├── scripts/            # Setup & migration scripts
├── logs/               # (gitignored)
├── README.md
├── .env.example
├── requirements.txt          # Python
