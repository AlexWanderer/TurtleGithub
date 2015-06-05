<?php
//adds a movie to a database
	if(isset($_POST['cancel']))
	{
		header("Location: hw6_baseForm.php");
		die();
	}
	include 'creds.php';	
	
	$title = $_POST["title"];
	$year = $_POST["year"];
	$studio = $_POST["studio"];
	$price = $_POST["price"];

	$GLOBALS['msg'] = "</br>";//reset due to null checks
	
	$success = true;
	$success &= validateTitle($title);
	$success &= validateYear($year);	
	$success &= validatePrice($price);

	
	if($success)
	{
		$con = mysql_connect($host, $user, $pw);	
		mysql_select_db($db, $con);
		
		$sql1 = "INSERT INTO `movies`.`Movie` (`title`, `year`, `studio`, `price`) VALUES ('$title', $year, '$studio', $price);";

		//$sql1 = "INSERT INTO `movies`.`Movie` (`Title`, `Year`, `Studio`, `Price`) VALUES ('Animal House', '1978', 'Universal', '21.49');";
		
		if(mysql_errno())
		{
			echo "failed to connect</br>";	
		}
		else
		{
			if(mysql_query($sql1))
			{
				echo "Your data was saved to the database successfully!</br>";			
			}
			else
				echo "query unable to save</br>" . mysql_error();
		}
				
		mysql_close($con);
			
		header("Location: hw6_baseForm.php");
		die();
	}
	else
	{
		echo "</br>database could not validate the data";
		echo $GLOBALS['msg'] . "</br>";
	}
	
	function validateYear($n)
	{		
		if(is_numeric($n) && strlen($n . "") == 4)//8 digits //preg_match('/^\d{8}$/', $n)
		{
			return true;
		}
		else
		{			
			$GLOBALS['msg'] = $GLOBALS['msg'] . "Invalid Year</br>";
			return false;
		}		
	}
	function validateTitle($n)
	{				
		if(empty($n))//make sure no digits. 
		{
			$GLOBALS['msg'] = $GLOBALS['msg'] . "Invalid Title</br>";
			return false;
		}			
		else 
			return true;
	}
	function validatePrice($n)
	{				
		if(empty($n))//make sure no digits. 
		{
			$GLOBALS['msg'] = $GLOBALS['msg'] . "Invalid Price</br>";
			return false;
		}			
		else 
			return true;
	}
?>