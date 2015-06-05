<?php
//displays a list of the items in `company` table followed by 2 buttons and a checkbox 
//the checkbox sets on or off based on data from the database
$result = mysql_query("SELECT * FROM `company`");
?>
<h3><u>Company list.</u></h3>
<table border="1px">
    <?php while($row = mysql_fetch_array($result)) : ?>
        <tr>
            <td><?php echo $row['company_name']; ?></td>
            <td>
                <form action="" method="post">
                    <input type="hidden" name="company_id" value="<?php echo $row['company_id']; ?>" />
                    <input type="submit" value="Edit" />
                </form>
                <form action="deletecompany.php" method="post">
                    <input type="hidden" name="company_id" value="<?php echo $row['company_id']; ?>" />
                    <input type="submit" value="Delete" />
                </form>
                <input type="checkbox" name="allow_email" data-cid="<?php echo $row['company_id']; ?>" <?php if (mysql_result(mysql_query("SELECT COUNT(company_id) FROM `company` WHERE `company_id` = ". $row['company_id'] . " AND `can_message` = 1;"), 0) == 1){    echo 'checked=checked';}?>>Allow Email
            </td>

        </tr>
    <?php endwhile;
    ?>
    <td>
        <form action="addcompany.php" method="">
            <input type="submit" name="add" value="Add Company" />
        </form>
    </td>
</table>

<script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
<script>
    $(document).ready(function()
    {

        $("input[type='checkbox']").on('click', function()
        {
            var checked = $(this).prop('checked');
            if(checked)
            {
                //$.post('updatesubscription.php', { cid:$(this).attr('data-cid'), checked:true }, function(data){
                $.post('updatecompany.php', { co_id:$(this).attr('data-cid'), checked: '1' }, function(data)
                {
                });
            }
            else
            {
                console.log('nope');
                $.post('updatecompany.php', { co_id:$(this).attr('data-cid'), checked:'0' }, function(data){});
            }
        });

    });
</script>