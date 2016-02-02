
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.fxml.FXML;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.TextField;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.stage.Stage;
import java.util.ArrayList;

/**
 * Created by Turtle on 8/13/2015.
 * controller for creation of new table (shows temporary table and asks table name)
 */

public class Controller_Create
{
    @FXML
    TableView<dummydata> browse_table;
    @FXML
    TableColumn<dummydata, String> c1, c2;
    @FXML
    TextField db_name;
    public Controller_Scour sc;//ref to previous window controller


    ObservableList<dummydata> d;
    @FXML
    public void initialize()
    {
        //load in temp table data
        //System.out.println("dastext: " + sc.dir_text.getText());

        if(sc.dir_text.getText().isEmpty() != true)
        {
            String[] exts = {".pdf"};
            ArrayList<String> retrievedFileNames = Utils.getFileNames(sc.dir_text.getText(), true, exts);// cb_subdir.isSelected()
            d =  FXCollections.<dummydata>observableArrayList();
            for (String s : retrievedFileNames)
            {
                //System.out.println(s);
                d.add(new dummydata(s, "ext", true));
            }
            c1.setCellValueFactory(new PropertyValueFactory<dummydata, String>("_firstname"));
            browse_table.setItems(d);
            //browse_table.setItems(generateDummyData());

            //todo THIS fucking string data crap doesnt work T_T
            //todo inefficient data passing: first i have a string from getFileNames, turn that into dummydata list, then turn in to node list, pass list to db_manager
            /*
            ObservableList<String> oFileNames = FXCollections.observableArrayList(retrievedFileNames);
            browse_table.setItems(oFileNames);
            c1.setCellValueFactory(new Callback<TableColumn.CellDataFeatures<ObservableList<String>, String>, ObservableValue<String>>()
            {
                @Override
                public ObservableValue<String> call(TableColumn.CellDataFeatures<ObservableList<String>, String> param) {
                    return new SimpleStringProperty(param.getValue().get(0));
                }
            });
            */
        }

    }

    public void onCreateButton()
    {
        //call database(db_name, nodelist)
        ObservableList<Node> finalData = FXCollections.<Node>observableArrayList();
        for(dummydata dd : d)
        {
            if(dd.isKeep())
            {
                finalData.add(new Node(dd.get_firstname(), dd.get_extension(), "", false, "path", "", ""));
            }
        }
        DB_Manager.getInstance().createTable(db_name.getText(), finalData);

        //close create window
        Stage createStage = (Stage) browse_table.getScene().getWindow();
        createStage.close();
    }

    private ObservableList<Node> generateDummyData()
    {
        ObservableList<Node> nodes = FXCollections.<Node>observableArrayList();
        Node n1 = new Node("Effective C++", "C++");
        Node n2 = new Node("Programming Practice and Principles", "C++");
        Node n3 = new Node("Advanced C++", "C++");

        nodes.addAll(n1, n2, n3);
        return nodes;
    }
}
