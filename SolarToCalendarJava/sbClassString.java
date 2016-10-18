/*
 * All Code Copyright Frank Migliorino © 2013.
 */ 

public class sbClassString {
    public String name;
    public String number;
    public String section;
    public String component;
    public String dateTime;
    public String location;
    public String instructors;
    public String startEndDate;
    public String fullName() {
        return name + " (" + component + " " + section + ")";
    }
    public String toString() {
        return fullName();
    }
}
