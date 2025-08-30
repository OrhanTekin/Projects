const fs = require('fs') //Filesystem

function readDb(dbName = 'db.json') {
    // read JSON object from file
    const data = fs.readFileSync(dbName, 'utf8')
    return JSON.parse(data)
}

function writeDb(obj, dbName = 'db.json') {
    if (!obj) return console.log('Please provide data to save')
    try {
        let currentData = {}
        if (fs.existsSync(dbName)) {
            currentData = readDb(dbName)
        }

        // Merge current data with new data (shallow merge)
        const mergedData = { ...currentData, ...obj }

        //Überschreibe nur angegebenen Daten
        fs.writeFileSync(dbName, JSON.stringify(mergedData, null, 2))
        return console.log('SAVE SUCCESS')
    } catch (err) {
        console.error(err)
        return console.log('FAILED TO WRITE')
    }
}



module.exports = { readDb, writeDb }