/*
 * All Code Copyright Frank Migliorino © 2013.
 */ 
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.GregorianCalendar;
import java.util.Random;

import javax.swing.JFileChooser;

public class performRead {
	
	static Random rndGen = new Random();
	
	static final String HEAD1 = "BEGIN:VCALENDAR\r\n" + 
			"VERSION:2.0\r\n" + 
			"METHOD:PUBLISH";
	
	static final String HEAD2 = "BEGIN:VTIMEZONE\r\n" + 
			"TZID:Eastern Standard Time\r\n" + 
			"BEGIN:STANDARD\r\n" + 
			"DTSTART:16011104T020000\r\n" + 
			"RRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11\r\n" + 
			"TZOFFSETFROM:-0400\r\n" + 
			"TZOFFSETTO:-0500\r\n" + 
			"END:STANDARD\r\n" + 
			"BEGIN:DAYLIGHT\r\n" + 
			"DTSTART:16010311T020000\r\n" + 
			"RRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3\r\n" + 
			"TZOFFSETFROM:-0500\r\n" + 
			"TZOFFSETTO:-0400\r\n" + 
			"END:DAYLIGHT\r\n" + 
			"END:VTIMEZONE";
	
	
	static void readText(String input) {
		
		// S1: Convert Clipboard
		
		String rawInput = input;
        ArrayList<sbClassString> classListStr = new ArrayList<sbClassString>();
        ArrayList<String> tempSplitInput = new ArrayList<String>();
        for (String x: rawInput.split("\\r?\\n")) {
        	 tempSplitInput.add(x);
        }
        for (int i = 0; i < tempSplitInput.size(); i++) {
        	if (tempSplitInput.get(i).equals("")) {tempSplitInput.remove(i--);}
        }
        for (int x = 0; x < tempSplitInput.size(); x++) {
            if (tempSplitInput.get(x).equals("\t")) {
                tempSplitInput.remove(x);
                x--;
            }
        }
        
        String[] splitInput = new String[tempSplitInput.size()];
        for (int x = 0; x < tempSplitInput.size(); x++) {
        	splitInput[x] = tempSplitInput.get(x);
        }
        
        // string[] splitInput = rawInput.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        int i = 0;
        String currentClassName = splitInput[i++];
        // credit to tchrist on StackOverflow for this:
        String countTabs = splitInput[i++];
        String countTabsSmaller = countTabs.replace("\t", "");
        int numTabs = countTabs.length() - countTabsSmaller.length() + 2;
        i += numTabs;
        // end credit
        boolean onlineCourse = false;

        while (i < splitInput.length) {

            sbClassString thisClass = new sbClassString();
            thisClass.name = currentClassName;
            thisClass.number = (splitInput[i++]);
            if (thisClass.number.trim().isEmpty()) {
                i--;
                onlineCourse = true;
            }
            thisClass.section = splitInput[i++];
            thisClass.component = splitInput[i++];
            thisClass.dateTime = splitInput[i++];
            // SPECIAL CASE - DateTime will not be here if online course
            if (thisClass.section.toLowerCase().contains("online") || thisClass.component.toLowerCase().contains("online")
            		|| thisClass.dateTime.trim().isEmpty()) {
                i--;
                onlineCourse = true;
            }
            // case: If the 
            thisClass.location = splitInput[i++];
            //splitInput[i] = splitInput[i].substring(1);
            thisClass.instructors = "";
            while (true) {
                thisClass.instructors += splitInput[i++];
                if (Character.isDigit(splitInput[i].charAt(0))) {break;} //no more inst's
            }
            thisClass.startEndDate = splitInput[i++];
            if (!onlineCourse) { classListStr.add(thisClass); }
            onlineCourse = false;
            if (i >= splitInput.length) { break; }
            if (Character.isDigit(splitInput[i].charAt(0)) || splitInput[i].equals(" ")) {
                // another component, same name.
                continue;
            } else {
                currentClassName = splitInput[i++];
                // credit to tchrist on StackOverflow for this:
                countTabs = splitInput[i++];
                countTabsSmaller = countTabs.replace("\t", "");
                numTabs = countTabs.length() - countTabsSmaller.length() + 2;
                i += numTabs;
                // end credit
            }
        }
        i = 0;
        // All Done Parsing!

		JFileChooser fc = new JFileChooser();
		// Save section
        int returnVal = fc.showSaveDialog(null);
        if (returnVal != JFileChooser.APPROVE_OPTION) {
        	return;
        }
        File file = fc.getSelectedFile();
        try ( PrintWriter fout
        		   = new PrintWriter (new BufferedWriter(new FileWriter(file.getPath() + ".ics")));) {
        
        	fout.println(HEAD1);
        	fout.println("X-WR-CALNAME:" + file.getName());
        	fout.println(HEAD2);
        	
        	for(sbClassString xClass: classListStr) {
                fout.print("BEGIN:VEVENT\nCLASS:PUBLIC\nPRIORITY:5\nSEQUENCE:0\n" +
                        "STATUS:CONFIRMED\nTRANSP:OPAQUE\nX-MICROSOFT-CDO-BUSYSTATUS:BUSY\nUID:");
                fout.println(rndGen.nextInt());
                // Getting the time of the class

                String tempDTS = xClass.startEndDate.substring(0,0+ xClass.startEndDate.indexOf(" "));
                String tempDTE = xClass.startEndDate.substring(xClass.startEndDate.indexOf("-") + 2);
                String tempDaysTimes = xClass.dateTime;
                String tempTimes = tempDaysTimes.substring(tempDaysTimes.indexOf(" ") + 1);
                // Assumptions made on copied schedule:
                // 1) Date Start is Monday
                // so to get the dates for classes, take the first day offered, and use
                // that to offset from that starting day.
                int offset = 0;
                switch (tempDaysTimes.substring(0,0+2)) {
                    case "Mo":
                        offset = 0;
                        break;
                    case "Tu":
                        offset = 1;
                        break;
                    case "We":
                        offset = 2;
                        break;
                    case "Th":
                        offset = 3;
                        break;
                    case "Fr":
                        offset = 4;
                        break;
                    case "Sa":
                        offset = 5;
                        break;
                    case "Su":
                        offset = 6;
                        break;
                }
                int hour = Integer.parseInt(tempTimes.substring(0, 0+tempTimes.indexOf(":") - 0));
                int min = Integer.parseInt(tempTimes.substring(tempTimes.indexOf(':') + 1,
                		tempTimes.indexOf(':') + 1+ 2));
                if (tempTimes.charAt(tempTimes.indexOf("-") - 3) == 'P' && hour != 12) { hour += 12; }
                
                TimeSpan tStart = new TimeSpan(hour, min);
                
                
                tempTimes = tempTimes.substring(tempTimes.indexOf("-") + 2);
                
                hour = Integer.parseInt(tempTimes.substring(0, tempTimes.indexOf(":") - 0));
                min = Integer.parseInt(tempTimes.substring(tempTimes.indexOf(':') + 1,
                		tempTimes.indexOf(':') + 1+ 2));
                if (tempTimes.charAt(tempTimes.indexOf(":") + 3) == 'P' && hour != 12) { hour += 12; }
                TimeSpan tEnd = new TimeSpan(hour, min);

               // DateTime classStarts2 = DateTime.Parse(tempDTS);
                
                DateTime base = DateTime.Parse(tempDTS).AddDays(offset);
                DateTime base2 = new DateTime( (GregorianCalendar) (base.getIntern().clone()));
                
                
                DateTime classStarts = base.Add(tStart);
                DateTime classEnds = base2.Add(tEnd);
                DateTime classReallyEnds = DateTime.Parse(tempDTE);

                fout.print("DTSTART;TZID=\"Eastern Standard Time\":");
                fout.println(classStarts.toString(""));
                fout.print("DTEND;TZID=\"Eastern Standard Time\":");
                fout.println(classEnds.toString(""));

                String daysRepeated = tempDaysTimes.substring(0, tempDaysTimes.indexOf(" ") + 1).toUpperCase();
                {
                	ArrayList<Character> daysRep = new ArrayList<Character>();
                	for(char c: daysRepeated.toCharArray()) {
                		daysRep.add(c);
                	}
                	int j = 0;
                    while (j < daysRep.size()) {
                        j += 2;
                        if (j < daysRep.size() - 1) {
                            daysRep.add(j, ',');
                            j++;
                        }
                    }
                    daysRep.remove(daysRep.size() - 1);
                    StringBuilder daysRepeatedTemp = new StringBuilder();
                    for(Character c: daysRep) {
                    	daysRepeatedTemp.append(c.charValue());
                    }
                    daysRepeated = new String(daysRepeatedTemp);
                }

                fout.println("RRULE:FREQ=WEEKLY;UNTIL=" + classReallyEnds.toString() + "T223000;BYDAY=" + daysRepeated);
                fout.print("DESCRIPTION:");
                fout.println(String.format("%s\\nClass Number: %s\\nClass Section: %s\\nComponent: %s\\nDateTime: %s" +
                    "\\nLocation: %s\\nInstructors: %s\\nStart and End Date: %s",
                    xClass.fullName(), xClass.number, xClass.section,
                        xClass.component, xClass.dateTime, xClass.location, xClass.instructors, xClass.startEndDate
                    ));
                fout.println("LOCATION:" + xClass.location);
                fout.println("SUMMARY:" + xClass.fullName());
                fout.println("END:VEVENT");

        	}
            fout.println("END:VCALENDAR");
            fout.flush();
            fout.close();
        	
        } catch (IOException x) {
            System.err.format("IOException: %s%n", x);
        }

	}
}
