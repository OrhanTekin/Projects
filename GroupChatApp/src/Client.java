import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

public class Client implements Runnable{

	private Socket client;
	private BufferedReader in;
	private PrintWriter out;
	private boolean terminated;
	
	
	@Override
	public void run() {
		try {
			//connection to local host - change ip here to server host ip
			client = new Socket("127.0.0.1", 9999);
			out = new PrintWriter(client.getOutputStream(), true);
			in = new BufferedReader(new InputStreamReader(client.getInputStream()));
			
			InputHandler inputHandler = new InputHandler();
			Thread t = new Thread(inputHandler);
			t.start();
			
			String inputMessage;
			while((inputMessage = in.readLine()) != null) {
				//print incoming messages from server to this client
				System.out.println(inputMessage);
			}
		}catch(IOException e) {
			//handle
			disconnect();
		}
		
	}
	
	public void disconnect() {
		terminated = true;
		try {
			in.close();
			out.close();
			if(!client.isClosed()) {
				client.close();
			}
		}catch(IOException e) {
			//
		}
	}
	
	class InputHandler implements Runnable{

		@Override
		public void run() {
			try {
				BufferedReader inputReader = new BufferedReader(new InputStreamReader(System.in));
				while(!terminated) {
					String message = inputReader.readLine();
					out.println(message);					
					if(message.equals("/quit")) {						
						inputReader.close();
						disconnect();
					}
				}
			}catch(IOException e) {
				//handle
				disconnect();
			}			
		}		
	}

	public static void main(String[] args) {
		Client client = new Client();
		client.run();
	}
	
}
