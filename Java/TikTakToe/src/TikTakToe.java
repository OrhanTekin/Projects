import java.util.Scanner;

public class TikTakToe {

    private static char currentPlayer;
    private static char[][] board;
    private static boolean gameEnded;

    public static void main(String[] args) throws Exception {
        startGame();
        Scanner in = new Scanner(System.in);

        while(!gameEnded){
            printBoardState();
            playerInput(in);
            checkGameOver();
            switchPlayer();
        }
        printBoardState();
    }

    /**
     * Initialize the game
     */
    public static void startGame(){
        currentPlayer = 'X';
        board = new char[][]{{' ', ' ', ' '},{' ', ' ', ' '},{' ', ' ', ' '}};
        gameEnded = false;
    }

    /**
     * Print current state of the board
     */
    public static void printBoardState(){
        System.out.println("Board: ");
        System.out.println("");
        System.out.println("-------------");

        for(int i = 0; i<3; i++){ 
            System.out.print("| ");
            for(int j = 0; j<3; j++){
                System.out.print(board[i][j]);
                System.out.print(" | ");
            }
            System.out.println("");
            System.out.println("-------------");
        }
        System.out.println("");
    }

    /**
     * Take user input and insert into the board if the move is allowed
     * @param in Scanner
     */
    public static void playerInput(Scanner in){
        int row, column;
        do{
            System.out.println("Player " + currentPlayer + "'s turn. Enter row and column (1-3): ");
            row = in.nextInt();
            column = in.nextInt();
        }while((row <= 0 || row > 3) || (column <= 0 || column > 3));

        if(board[row-1][column-1] == ' '){
            board[row-1][column-1] = currentPlayer;
        }else{
            System.out.println("That field is already filled");
            playerInput(in);
        }
    }

    /**
     * Check if a player won the game with the last move
     */
    public static void checkGameOver(){

        for(int i = 0; i<3; i++){
            if(board[i][0] == board[i][1] && board[i][1] == board[i][2] && board[i][0] != ' '){
                gameOver();
                break;
            }

            if(board[0][i] == board[1][i] && board[1][i] == board[2][i] && board[0][i] != ' '){
                gameOver();
                break;
            }
        }
        if(board[1][1] != ' '){
            if((board[0][0] == board[1][1] && board[1][1] == board[2][2]) || (board[0][2] == board[1][1] && board[1][1] == board[2][0])){
                gameOver();
            }
        }
    }

    public static void switchPlayer(){
        currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
    }

    public static void gameOver(){
        System.out.println("The game is over! Player " + currentPlayer + " won");
        gameEnded = true;
    }

}
