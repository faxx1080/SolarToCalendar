/*
 * All Code Copyright Frank Migliorino © 2013.
 */ 
import java.awt.BorderLayout;
import java.awt.EventQueue;
import java.awt.Font;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JSplitPane;
import javax.swing.JTextArea;
import javax.swing.UIManager;
import javax.swing.UnsupportedLookAndFeelException;



public class MainForm {

	private JFrame frame;

	/**
	 * Launch the application.
	 */
	public static void main(String[] args) {
		EventQueue.invokeLater(new Runnable() {
			public void run() {
				
		        try {
					UIManager.setLookAndFeel(
					        UIManager.getSystemLookAndFeelClassName());
				} catch (ClassNotFoundException e1) {
				} catch (InstantiationException e1) {
				} catch (IllegalAccessException e1) {
				} catch (UnsupportedLookAndFeelException e1) {
				}
				
				try {
					MainForm window = new MainForm();
					window.frame.setVisible(true);
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		});
	}

	/**
	 * Create the application.
	 */
	public MainForm() {
		initialize();
	}

	/**
	 * Initialize the contents of the frame.
	 */
	private void initialize() {
		frame = new JFrame();
		frame.setBounds(100, 100, 478, 459);
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.getContentPane().setLayout(new BorderLayout(0, 0));
		
		JPanel panel = new JPanel();
		frame.getContentPane().add(panel, BorderLayout.NORTH);
		
		
		final JTextArea txtrTxtinst = new JTextArea(10,40);
		txtrTxtinst.setFont(new Font("Arial", Font.PLAIN, 12));
		txtrTxtinst.setText("Directions:\r\n\r\n0) You must use either **Firefox** or **Chrome** for this to work.\r\n1) Open your solar home page.\r\n2) Go to Student Records and Registration.\r\n3) Go to Enrollment.\r\n4) Go to My Class Schedule (NOT My Weekly Schedule)\r\n5) Select from the first character of the first class listed on the page. For example, if it said \"ITS 101 - Int...\", you would start selecting from that first I to...\r\n6) ...the last digit in the last table with a class. This is often the date that is right above the link that says \"Printer Friendly Page\". Select from the first class (see #5) to the last digit of that date.\r\n7) Copy all that.\r\n8) Click the button that says Make Calendar (real Schedule).\r\n\r\nThis will (hopefully) create and ask you to save an ICS file - a digital calendar file. It contains all the information on solar that you copied (Except for the # of credits, I may update later for that). You can import this file to places like Apple's Calendar and Google Calendar, and ta-da you and your smartphone if you have one now have a one-click away map of your entire week! Where you have to be, when you have to be there, and with more accuracy than Solar's \"grid view\" which rounds to the nearest hour.\r\n\r\nI hope you enjoyed this,\r\nFrank Migliorino.\r\n\r\n\u00A9 Frank Migliorino 2013. All Rights Reserved.");
		
		txtrTxtinst.setWrapStyleWord(true);
		txtrTxtinst.setLineWrap(true);
		txtrTxtinst.setEditable(false);
		txtrTxtinst.setFocusable(false);
		txtrTxtinst.setOpaque(false);

		
		JScrollPane scrollPane = new JScrollPane(txtrTxtinst);
		panel.add(scrollPane);

		
		JSplitPane splitPane = new JSplitPane();
		splitPane.setResizeWeight(0.95);
		splitPane.setOrientation(JSplitPane.VERTICAL_SPLIT);
		frame.getContentPane().add(splitPane, BorderLayout.CENTER);
		
		final JTextArea inputField = new JTextArea();
		inputField.setFont(new Font("Monospaced", Font.PLAIN, 11));
		splitPane.setLeftComponent(inputField);
		
		JSplitPane splitPane_1 = new JSplitPane();
		splitPane_1.setResizeWeight(0.5);
		splitPane.setRightComponent(splitPane_1);
		
		JButton btnMakeCalendartentative = new JButton("Make Calendar (Tentative Sch.)");
		btnMakeCalendartentative.setEnabled(false);
		btnMakeCalendartentative.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
			}
		});
		splitPane_1.setLeftComponent(btnMakeCalendartentative);
		
		JButton btnMakeCalendarreal = new JButton("Make Calendar (real schedule)");
		btnMakeCalendarreal.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				try {
					performRead.readText(inputField.getText());
				} catch (Exception e1) {
					txtrTxtinst.setText("An error has occured. Restart the app, or tell me via email.");
					return;
				}
				txtrTxtinst.setText("Success! To import to Google Calendar:\n"
						+ "1) Go to Google Calendar, and sign in with either your Google Account or Stony Brook's.\r\n" + 
						"2) Once loaded, make a new calendar (if you want): Near My Calendars, click that small dropdown arrow, and click create a new calendar. Fill in whatever you want.\r\n" + 
						"3) Optional: If you want to share your schedule with others, I recommend you use the Stony Brook account - everyone's names will autocomplete. Under\"\"Share this Calendar\"\" in calendar details, add the names of people you want to see your calendar. That's it.\r\n" + 
						"4) Back at calendar.google.com, under Other Calendars, click the arrow that has a dropdown menu with \"\"Import calendar\"\" - this will pop up a dialog asking for that file this program just made. Choose that file, and choose the calendar you want your schedule to go in.\r\n" + 
						"5) Enjoy! If you want this calendar on your smartphone, you can find that on google.");
			}
		});
		splitPane_1.setRightComponent(btnMakeCalendarreal);
	}
}

