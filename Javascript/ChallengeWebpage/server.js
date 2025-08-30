const express = require('express')
const bodyParser = require('body-parser')
const cors = require('cors')
const { readDb, writeDb } = require('./dbFunctions')

const app = express()
app.use(cors())
app.use(bodyParser.json())

// Get list of games
app.get('/games', (req, res) => {
  const db = readDb()
  res.json(db.games || [])
})

// Save updated games list
app.post('/games', (req, res) => {
  const games = req.body.games
  if (!Array.isArray(games)) return res.status(400).json({ error: "games must be an array" })

  writeDb({ games })
  res.json({ success: true })
})

// ✅ Use PORT from environment (important for Render!)
const PORT = process.env.PORT || 3000
app.listen(PORT, () => console.log(`Server running on port ${PORT}`))
