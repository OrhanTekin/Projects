const startBtn = document.querySelector("#start"),
    screens = document.querySelectorAll(".screen"),
    timeEl = document.querySelector("#time"),
    board = document.querySelector("#board"),
    hitsEl = document.querySelector("#hits"),
    accuracyEl = document.querySelector("#accuracy"),
    hitsOver = document.querySelector("#hits-over"),
    accuracyOver = document.querySelector("#accuracy-over"),
    timeOver = document.querySelector("#time-over"),
    healthEl = document.querySelector("#health"),
    restartBtn = document.querySelector("#restart"),
    fullScreenBtn = document.querySelector("#fullscreen"),
    highscoreEl = document.querySelector("#highscore"),
    speaker = document.querySelector("#speaker");

let time = 0,
    playing=false,
    hits=0,
    missed=0, 
    accuracy=0,
    interval,
    circleInterval,
    spawnInterval,
    health=3,
    audio,
    toogleMute;


startBtn.addEventListener("click", () => {
    screens[0].classList.add("up");
    setup();
    startGame();
});


function setup(){
    //restart button
    restartBtn.addEventListener("click", restartGame);
    

    //event on circle click
    board.addEventListener("mousedown", (e) =>{
        if(e.target.classList.contains("circle")){
            //increase hits by one and remove circle
            hits++;
            e.target.remove();
            if(!toogleMute){
                audio.play();
            }
            
        }else{
            //missed circle
            missed++;
        }

        //update hits on stats
        hitsEl.innerHTML = hits;
        calculateAccuracy();
    });


    //fullscreen
    fullScreenBtn.addEventListener("click", () => {
        if(elem.requestFullscreen){
            elem.requestFullscreen();  
        }
        //hide fullscreen button
        fullScreenBtn.style.display = "none";
    });

    let elem = document.documentElement;

    document.addEventListener("fullscreenchange", () => {
        if(!document.fullscreenElement){
            fullScreenBtn.style.display = "block";
        }
    });

    //sound
    speaker.addEventListener("click", () => {
        if(!toogleMute){
            speaker.classList.add("mute");
            toogleMute = true;
        }else if(toogleMute){
            speaker.classList.remove("mute");
            toogleMute = false;
        }
        localStorage.clear("toogleMute");
        localStorage.setItem("toogleMute", toogleMute);

        
    });

    //load highscore
    const highscore = localStorage.getItem("myHighscore");
    if(highscore != null){
        highscoreEl.innerHTML = highscore;
    }
    //load audio and settings
    audio = new Audio('hitSound.mp3');
    toogleMute = localStorage.getItem("toogleMute");
    if(toogleMute){
        speaker.classList.add("mute");
    }

}


function startGame(){
   

    //start game
    playing = true;
    interval= setInterval(increaseTime, 1000);
    changeSpawnRate();
    spawnInterval = setInterval(changeSpawnRate, 5000);
}

function changeSpawnRate(){
    clearInterval(circleInterval);
    circleInterval = setInterval(createRandomCircle, 500-(time*7));
}


function increaseTime(){
    ++time;

    let minutes = Math.floor(time / 60 );
    let seconds = Math.floor(time % 60);

    //add trailing zero

    seconds= seconds < 10 ? "0" + seconds : seconds;
    minutes= minutes < 10 ? "0" + minutes : minutes;

    //set time
    timeEl.innerHTML = `${minutes}:${seconds}`;
}


function createRandomCircle(){
    if(!playing){
        return;
    }

    const circle = document.createElement("div");
    const size = 50;
    const {width, height} = board.getBoundingClientRect();
    const x = getRandomNumber(size, width-size);
    const y = getRandomNumber(size, height-size);
    circle.classList.add("circle");
    circle.style.width = `${size}px`;
    circle.style.height = `${size}px`;
    circle.style.top = `${y}px`;
    circle.style.left = `${x}px`;  

    board.append(circle);
 

    //add new circle when old one disappears
    circle.addEventListener("animationend", () =>{
        circle.remove();
        //increase misses also when circle scale becomes 0 and user didnt click in time
        addMissed();
    });

    
}


function finishGame(){
    playing = false;
    clearInterval(interval);
    clearInterval(circleInterval);
    clearInterval(spawnInterval);
    board.innerHTML = "";
    screens[1].classList.add("up");
    hitsEl.innerHTML = 0;
    accuracyEl.innerHTML = "100%";

    //update stats at end
    hitsOver.innerHTML = hits;
    accuracyOver.innerHTML = `${accuracy}%`;

    //set game over time
    timeOver.innerHTML = timeEl.innerHTML;

    //update highscore 
    let highscoreMinutes = parseInt(highscoreEl.innerHTML.substring(0,2));
    let highscoreSeconds = parseInt(highscoreEl.innerHTML.substring(3));

    let minutes = parseInt(timeOver.innerHTML.substring(0,2));;
    let seconds = parseInt(timeOver.innerHTML.substring(3));;

    if(minutes > highscoreMinutes || minutes == highscoreMinutes && seconds > highscoreSeconds){
        highscoreEl.innerHTML = timeOver.innerHTML;
        localStorage.clear("myHighscore");
        localStorage.setItem("myHighscore", highscoreEl.innerHTML);
    }

    //reset time html
    timeEl.innerHTML = "00:00";
}

function addMissed(){
    if(health == 1){
        finishGame();
    }else{
        health--;
        healthEl.innerHTML = health;
    }
}

function calculateAccuracy(){
    accuracy = (hits / (hits + missed)) * 100;
    accuracy = accuracy.toFixed(2);
    accuracyEl.innerHTML = `${accuracy}%`;
}

function getRandomNumber(min, max){
    return Math.round(Math.random()* (max-min) + min);
}



function restartGame(){
    screens[1].classList.remove("up");
    reset();
    startGame();
}

function reset(){
    time=0;
    hits=0;
    missed=0;
    accuracy=0;
    playing=false;
    health=3;
    healthEl.innerHTML = health;
}



function check(){
    if(window.performance.getEntriesByType('navigation')[0].type == 'reload'){
        screens[0].classList.add("up");
        screens[1].classList.add("up");
        setup();
    }
}
