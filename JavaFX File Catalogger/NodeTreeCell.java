import javafx.event.Event;
import javafx.event.EventHandler;
import javafx.scene.control.*;
import javafx.scene.input.KeyCode;
import javafx.scene.input.KeyEvent;

public class NodeTreeCell extends TreeCell<String>
{

    private TextField textField;
    private ContextMenu deleteMenu = new ContextMenu();

    public NodeTreeCell()
    {
        MenuItem addMenuItem = new MenuItem("Delete");
        deleteMenu.getItems().add(addMenuItem);
        addMenuItem.setOnAction(new EventHandler()
        {
            public void handle(Event t)
            {
                System.out.println("context menu clicked");
            }
        });
    }

    @Override
    public void startEdit()//called on a double click
    {
        super.startEdit();
        System.out.println("** startEdit() **");

        if (textField == null)
        {
            createTextField();
        }
        setText(null);
        setGraphic(textField);
        textField.selectAll();

    }

    @Override
    public void cancelEdit()//if you start to edit but dont change anything
    {
        super.cancelEdit();
        System.out.println("** cancelEdit() **");

        setText((String) getItem());
        setGraphic(getTreeItem().getGraphic());

    }

    @Override
    public void updateItem(String item, boolean empty)//called on all items if you update even a single item
    {
        super.updateItem(item, empty);
        System.out.println("** updateItem() **");

        if (empty)
        {
            setText(null);
            setGraphic(null);
        }
        else
        {
            if (isEditing())
            {
                if (textField != null)
                {
                    textField.setText(getString());
                }
                setText(null);
                setGraphic(textField);
            }
            else//right click
            {
                setText(getString());
                setGraphic(getTreeItem().getGraphic());
                setContextMenu(deleteMenu);
            }
        }

    }

    private void createTextField()
    {
        textField = new TextField(getString());
        textField.setOnKeyReleased(new EventHandler<KeyEvent>()
        {

            @Override
            public void handle(KeyEvent t)
            {
                if (t.getCode() == KeyCode.ENTER)
                {
                    commitEdit(textField.getText());
                } else if (t.getCode() == KeyCode.ESCAPE)
                {
                    cancelEdit();
                }
            }
        });

    }

    private String getString()
    {
        return getItem() == null ? "" : getItem().toString();
    }
}