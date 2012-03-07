<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>Free Food Camera</title>
	<meta http-equiv="refresh" content="10"/>
	<link runat="server" rel="shortcut icon" href="favicon.ico" type="image/x-icon"/>
    <link runat="server" rel="icon" href="favicon.ico" type="image/ico"/>
	<style type="text/css">
		body, div, p {
			font-family: "Calibri", sans-serif;
		}
		#content-frame {
			margin: 0 auto;
			width: 800px;
			text-align: center;
		}
	</style>
</head>

<body>
	<div id="content-frame">
		<h2 style="margin-bottom:5;">Free food in the DGP Lounge!</h2> 
		<h3 style="margin-top:0;margin-bottom:0;">Here is what's left right now:</h3>

		<img src="Food.jpg" class="food-image" name="image" height="600" width="800"/>

		Last updated 
		<span id="timeSec">
			<% 
				Dim objFSO, file, modified
				
				objFSO = CreateObject("Scripting.FileSystemObject")
				file = objFSO.GetFile("C:\inetpub\wwwroot\FreeFood\Food.jpg")

				Response.Write(datediff( "s" , file.DateLastModified, now( )))
			%>
		</span>
		<script type="text/javascript">
			var int=self.setInterval("clock()",1000);
			function clock()
			{
				var seconds = document.getElementById("timeSec").innerHTML;
				document.getElementById("timeSec").innerHTML = 1+parseInt(seconds);
			}
		</script>
		seconds ago.
	</div>
	<center style="margin-top:5;">To get notified of free food, sign up for the mailing list at <br/> <a href="http://www.dgp.toronto.edu/mailman/listinfo/freefood">http://www.dgp.toronto.edu/mailman/listinfo/freefood</a><center>
</body>
</html>