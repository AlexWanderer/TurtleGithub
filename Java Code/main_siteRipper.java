import java.io.*;
import java.net.*;

/*
 * Written by Sam Arutyunyan 2012
 * This program searches through a sequential set of web urls for image files and saves them to the hard drive. 
 * Disclaimer!! Use at your own risk. Do not use on websites that have copyrighted images or that forbid the use
 * of software such as this to automatically download their files.
 * 
 * example: for a set of pictures in the form {website.com/img001.jpg, website.com/img002.jpg, etc...}
 * set the fields to: 
  	urlStart = "website.com/img";
	extension = ".jpg";
	title = "MyPicture - "; - optional prefix name
	startNum = 1;
	endNum = 36; - whatever the last number in the set is
	String padding = "%03d";
	the final result will be: {MyPicture - 001.jpg, MyPicture - 002.jpg, etc...}
 
 */

public class main_siteRipper 
{
	
public static void main(String[] args) throws Exception
{
	String urlStart = "website.com/img";
	String extension = ".jpg";
	String title = "MyPicture - "; //name each file will be before extension or iteration number
	int startNum = 1;
	int endNum = 36;
	String padding = "%03d";
	String saveToLocation = "C:\\Users\\UserName\\Desktop\\siteRip\\";
	
	String finalName = "";//don't set this

	
	for (int mainCounter = startNum; mainCounter <= endNum; mainCounter++)
	{
		
		//wait 1 sec after each itereration
		wait(1);
		
		//*********--- Part 1: Get image url and name ---**********
		String mainIndexString = Integer.toString(mainCounter);		
		
		String imageUrl = urlStart + String.format(padding, mainCounter) + extension;		
		
		//if we're at an image url or not timing out.. *****
		HttpURLConnection check;
		URL checkUrl = new URL(imageUrl);
		check = (HttpURLConnection) checkUrl.openConnection();
		
		String contentTypeString = "";
				
		check.setReadTimeout(10000);//connect for 10 sec
		contentTypeString = check.getContentType();//check if type is image or text
		
		
		 //if we had a timeout, its possible to retry later
		if (contentTypeString == null || contentTypeString == "")
		{			
			System.out.println("timedOut: " + mainIndexString);
			continue;//move to next iteration of loop
		}
		
		//sometimes a .jpg url has type text instead of image, in which case this program can't save it
		 if(contentTypeString.indexOf("text") != -1)
		 {
			 System.out.println("Text Screen at:\n" + imageUrl);
			 continue;
		 }		 
		
		finalName = title + mainCounter + ".jpg";
		
		//*********--- Part 2: Save Image to folder ---**********		
		String destinationFile = saveToLocation + finalName;					
		
		SaveImage(imageUrl, destinationFile);
		
	}//end for(main counter
		
}//end main constructor
	
	public static void SaveImage(String imageUrl, String destinationFile) throws IOException 
	{	
		URL url = new URL(imageUrl);
		InputStream is = url.openStream();
		OutputStream os = new FileOutputStream(destinationFile);
		
		byte[] b = new byte[2048];
		int length;
		
		while ((length = is.read(b)) != -1)
		{
			os.write(b, 0, length);
		}
		
		is.close();
		os.close();	
		
	}
	
	public static void wait (int n)
	{
        
        long t0, t1;
        t0 =  System.currentTimeMillis();
        do
        {
            t1 = System.currentTimeMillis();
        }
        while ((t1 - t0) < (n * 1000));
    }

			
}//end main class
