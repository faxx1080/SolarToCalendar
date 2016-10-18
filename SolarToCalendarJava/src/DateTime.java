/*
 * All Code Copyright Frank Migliorino © 2013.
 */ 


import java.util.Date;
import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.GregorianCalendar;


public class DateTime {
	
	GregorianCalendar intern;
	
	public DateTime(GregorianCalendar inp) {
		intern = inp;
	}
	
	public GregorianCalendar getIntern() {
		return intern;
	}
	
	public static DateTime Parse(String tempDTS) {
		DateFormat format = new SimpleDateFormat( "MM/dd/yy" );
		Date date = new Date();
		try {
			date = format.parse(tempDTS);
		} catch (ParseException e) {
			
		}
		GregorianCalendar x = new GregorianCalendar();
		x.setTime(date);
		return new DateTime(x);
	}

	public DateTime AddDays(int offset) {
		
		intern.add(Calendar.DAY_OF_YEAR, offset);
		return this;
	}

	public DateTime Add(TimeSpan tStart) {
		intern.add(Calendar.MINUTE, tStart.getCal().get(Calendar.MINUTE));
		intern.add(Calendar.HOUR_OF_DAY, tStart.getCal().get(Calendar.HOUR_OF_DAY));
		return new DateTime(intern);
	}
	
	public String toString() {
		String year, month, day;
		year = String.format("%04d",intern.get(Calendar.YEAR));
		month = String.format("%02d",intern.get(Calendar.MONTH)+1);
		day = String.format("%02d",intern.get(Calendar.DAY_OF_MONTH));

		return year+month+day;
	}
	
	public String toString(String format) {
		// yyyyMMdd'T'HHmm'00'
		String year, month, day, hour, min;
		year = String.format("%04d",intern.get(Calendar.YEAR));
		month = String.format("%02d",intern.get(Calendar.MONTH)+1);
		day = String.format("%02d",intern.get(Calendar.DAY_OF_MONTH));
		hour = String.format("%02d",intern.get(Calendar.HOUR_OF_DAY));
		min = String.format("%02d",intern.get(Calendar.MINUTE));

		return year+month+day+"T"+hour+min+"00";
	}

}
