# Backend API – Digital Twin Greenhouse

This backend is a **Node.js (Express)** service providing API endpoints for data retrieval, control operations, and integration with the digital twin system.

---

## 📂 Folder Structure
```
controllers/       # Request handlers for different API endpoints
models/            # Data models or schema definitions
router/            # Route definitions mapping URLs to controllers
utils/             # Helper functions and shared utilities
app.js             # Express application configuration
server.js          # Server startup file
test_db.js         # Database connection/test script
package.json       # Dependencies and scripts
Jenkinsfile        # CI/CD pipeline configuration
global-bundle.pem  # Certificate bundle (do not commit real keys in production)
```

---

## ⚙️ Installation
```bash
# Install dependencies
npm install
```

---

## ▶️ Running the Server
**Development**
```bash
npm run dev
```

**Production**
```bash
npm start
```

---

## 🌐 API Endpoints (Examples)
| Method | Endpoint       | Description               |
|--------|---------------|---------------------------|
| GET    | /sensors      | Retrieve all sensors data |
| POST   | /sensors      | Add new sensor data       |

*(Full list in `router/` folder)*

---

## ⚠️ Environment Variables
Create a `.env` file based on `.env.example`:
```
PORT=8080
DB_URL=mongodb://localhost:27017/dbname
```

---

## 🧪 Testing
```bash
npm test_db
```

---

## 🔒 Security Notes
- Do not commit `.env` or private certificates.
- Use `.gitignore` to exclude sensitive files.