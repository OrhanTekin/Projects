import javax.swing.*;
import java.awt.*;
import java.awt.event.*;

public class TikTakToeGUI implements ActionListener {

    private static char currentPlayer = 'X';
    private static JButton[][] buttons = new JButton[3][3];

    TikTakToeGUI(){
        JFrame frame = new JFrame("Tik Tak Toe");
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        frame.setSize(300,300);
        frame.setLayout(new GridLayout(3, 3));

        for(int i = 0; i<buttons.length; i++){
            for(int j=0;j<buttons[0].length; j++){
                buttons[i][j] = new JButton();
                //buttons[i][j].setName(new StringBuilder("Button").append(i).toString());
                buttons[i][j].setFont(new Font("Arial", Font.PLAIN, 40));
                buttons[i][j].addActionListener(this);
                frame.getContentPane().add(buttons[i][j]);
                //i++;
            }
        }
        frame.setVisible(true);
        currentPlayer = 'X';
    }

    @Override
    public void actionPerformed(ActionEvent e) {
        JButton btn = (JButton)e.getSource();
        //System.out.println(btn.getName());
        if(btn.getText().equals("")){
            btn.setText(currentPlayer + "");
            checkWin();
            switchPlayer();
        }
    }

    public void checkWin(){
        for(int i = 0; i<3; i++){
            if(buttons[i][0].getText().equals(buttons[i][1].getText()) 
                && buttons[i][1].getText().equals(buttons[i][2].getText())
                && !buttons[i][0].getText().isEmpty()
            ){
                gameOver();
                return;
            }

            if(buttons[0][i].getText().equals(buttons[1][i].getText()) 
                && buttons[1][i].getText().equals(buttons[2][i].getText())  
                && !buttons[0][i].getText().isEmpty()
            ){
                gameOver();
                return;
            }
        }

        if(!buttons[1][1].getText().isEmpty()){
            if((buttons[0][0].getText().equals(buttons[1][1].getText())
                && buttons[1][1].getText().equals(buttons[2][2].getText()))  
                || (buttons[0][2].getText().equals(buttons[1][1].getText())  
                && buttons[1][1].getText().equals(buttons[2][0].getText()))
            ){
                gameOver();
            }
        }
    }

    public void gameOver(){
        for(int i = 0; i<buttons.length; i++){
            for(int j=0;j<buttons[0].length; j++){
                buttons[i][j].removeActionListener(this);           
            }
        }
    }

    public void switchPlayer(){
        currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
    }
    


    public static void main(String[] args) throws Exception {
        TikTakToeGUI gui = new TikTakToeGUI();
    }


}
