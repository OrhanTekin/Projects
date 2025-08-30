let currentGames = []

async function fetchGames() {
    try {
    const res = await fetch('http://localhost:3000/games')
    currentGames = await res.json()
    renderGames()
    } catch (err) {
    console.error("Could not fetch games:", err)
    }
}

function renderGames() {
    const list = document.getElementById('game-list')
    list.innerHTML = ''

    currentGames.forEach((game, idx) => {
    const li = document.createElement('li')
    if (game.checked) li.classList.add('checked')

    const checkbox = document.createElement('input')
    checkbox.type = 'checkbox'
    checkbox.checked = game.checked
    checkbox.onchange = () => toggleGame(idx, checkbox.checked)

    const label = document.createElement('label')
    label.textContent = game.name

    li.appendChild(checkbox)
    li.appendChild(label)
    list.appendChild(li)
    })
}

async function toggleGame(index, checked) {
    currentGames[index].checked = checked
    await fetch('http://localhost:3000/games', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ games: currentGames })
    })
    renderGames()
}

async function addGame() {
    const input = document.getElementById('new-game')
    const name = input.value.trim()
    if (!name) return

    // Add new game
    currentGames.push({ name, checked: false })

    // Save to backend
    await fetch('http://localhost:3000/games', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ games: currentGames })
    })

    input.value = '' // clear input
    renderGames()
}