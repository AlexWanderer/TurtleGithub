
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.value.ObservableValue;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.TextField;

import javafx.scene.control.CheckBox;
import javafx.stage.DirectoryChooser;

import javafx.stage.Stage;
import javafx.util.Callback;


import java.io.File;
import java.io.IOException;
import java.util.ArrayList;

/**
 * Created by Turtle on 8/7/2015.
 */

public class Controller_Scour
{
    @FXML    TextField extension_text;
    @FXML    public TextField dir_text;

    @FXML CheckBox cb_subdir;

    Scene browseScene, createScene;//todo for back button support?

    public void onBrowseButton() throws IOException
    {
        //open scour window
        final DirectoryChooser directoryChooser = new DirectoryChooser();
        final File selectedDirectory = directoryChooser.showDialog(TurtleDB_Main.getPrimaryStage());

        if (selectedDirectory != null)
        {
            dir_text.setText(selectedDirectory.toString());
        }

    }
    public void onScourButton()//currently in browseScene
    {
        //save current scene (for back button)
        browseScene = extension_text.getScene();
        //Stage scourStage = (Stage) extension_text.getScene().getWindow();
        //scourStage.close();

        Stage stage = (Stage)browseScene.getWindow();
        //open creation window
        if(createScene == null)
        {
            FXMLLoader fxmlLoader = new FXMLLoader(getClass().getResource("CreateDB_UI.fxml"));
            Controller_Create c = new Controller_Create();
            c.sc = this;
            fxmlLoader.setController(c);
            Parent root1 = null;
            try {
                root1 = (Parent) fxmlLoader.load();
                //browse_table = (TableView) root1.lookup("#browse_table");
            } catch (IOException e)
            {
                e.printStackTrace();
                System.exit(1);
            }
            createScene = new Scene(root1);
            stage.setScene(createScene);
            //stage.showAndWait();
        }
        else
        {
            stage.setScene(createScene);
        }

    }


}
