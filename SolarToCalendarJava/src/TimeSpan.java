/*
 * All Code Copyright Frank Migliorino © 2013.
 */ 


import java.util.Calendar;
import java.util.GregorianCalendar;


public class TimeSpan {

	GregorianCalendar intern;
	
	public TimeSpan(int hrs, int mins) {
		intern = new GregorianCalendar();
		
		intern.set(Calendar.HOUR_OF_DAY, hrs);
		intern.set(Calendar.MINUTE, mins);
	}
	
	public GregorianCalendar getCal() {
		return intern;
	}
	
	public String toString() {
		return intern.toString();
	}
	
}
