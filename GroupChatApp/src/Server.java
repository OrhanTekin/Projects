import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class Server implements Runnable {

	private ArrayList<ConnectionHandler> connections;
	private ServerSocket server;
	private boolean terminated;
	private ExecutorService clientThreads;
	
	public Server() {
		connections = new ArrayList<>();
		terminated = false;
	}
	
	@Override
	public void run() {
		try {
			server = new ServerSocket(9999);
			clientThreads = Executors.newCachedThreadPool();
			while(!terminated) {
				Socket client = server.accept();
				ConnectionHandler handler = new ConnectionHandler(client);
				connections.add(handler);
				clientThreads.execute(handler);
			}
		} catch (Exception e) {
			//handle
			shutdown();
		}		
	}

	public void broadcast(String message) {
		for(ConnectionHandler ch : connections) {
			if(ch != null) {
				ch.sendMessage(message);
			}
		}
	}
	
	public void shutdown() {
		try {
			terminated = true;
			clientThreads.shutdown();
			if(!server.isClosed()) {
				server.close();
			}
			for(ConnectionHandler ch : connections) {
				ch.disconnect();	
			}			
		}catch(IOException e) {
			//cant handle
		}
	}
	
	class ConnectionHandler implements Runnable{

		private Socket client;
		private BufferedReader in;
		private PrintWriter out;
		private String nickname;
		

		public ConnectionHandler(Socket client) {
			this.client = client;
		}
		
		@Override
		public void run() {
			try {
				out = new PrintWriter(client.getOutputStream(), true);
				in = new BufferedReader(new InputStreamReader(client.getInputStream()));
				out.println("Please enter a nickname");
				nickname = in.readLine();
				broadcast(nickname + " joined the chat");
				String message;
				while((message = in.readLine()) != null) {
					if(message.startsWith("/nick ")) {
						//handle change nickname
						String[] splitCommand = message.split(" ", 2);
						if(splitCommand.length == 2) {
							broadcast(nickname + " renamed to " + splitCommand[1]);
							nickname = splitCommand[1];						
						}else {
							out.println("No nickname was provided");
						}
					}else if(message.equals("/quit")) {
						//quit
						broadcast(nickname + " left the chat!");
						disconnect();
					}else {
						broadcast(nickname + ": " + message);
					}
				}
			}catch(IOException e){
				//handle
				disconnect();
			}			
		}
		
		public void sendMessage(String message) {
			out.println(message);
		}
		
		public void disconnect() {
			try {
				in.close();
				out.close();
				if(!client.isClosed()) {
					client.close();
				}
			}catch(IOException e) {
				//cant handle
			}
		}
	}
	
	public static void main(String[] args) {
		Server server = new Server();
		server.run();
	}
}
