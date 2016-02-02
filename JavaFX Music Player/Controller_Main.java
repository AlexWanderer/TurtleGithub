import com.sun.jndi.toolkit.url.Uri;
import javafx.application.Platform;
import javafx.beans.InvalidationListener;
import javafx.beans.Observable;
import javafx.beans.value.ChangeListener;
import javafx.beans.value.ObservableValue;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.layout.BorderPane;
import javafx.scene.media.Media;
import javafx.scene.media.MediaPlayer;
import javafx.scene.text.Text;
import javafx.stage.Modality;
import javafx.stage.Stage;
import javafx.util.Duration;

import java.io.File;
import java.io.IOException;
import java.net.MalformedURLException;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

/**
 * Created by Sam Arutyunyan on 11/2/2015.
 */
public class Controller_Main
{
    public Controller_Main(){}

    DB_Manager dbm;
    @FXML
    ListView<String> lv_menu;
    @FXML
    ListView<String> lv_group;
    @FXML
    ListView<String> lv_music;
    @FXML
    BorderPane bp;

    @FXML Button bt_delete;

    @FXML
    Slider volumeSlider;
    @FXML
    Slider timeSlider;
    @FXML
    Text playTime;
    @FXML
    Text playDuration;
    List<MediaFile> ls;//holds temporarily retrieved files
    int curGroup;//0 = music, 1 = playlist
    MediaPlayer mp;
    Duration duration;
    @FXML
    public void initialize()
    {
        dbm = DB_Manager.getInstance();
        dbm.init();//shutdown in main scene.onclose (can't init in main)
        bt_delete.setDisable(true);
        lv_group.setVisible(false);
        lv_music.setTranslateX(-250.0);
        //connect listview to database?
        lv_menu.getItems().addAll("Music", "Artist", "Genre", "Playlists");
        lv_menu.getSelectionModel().selectedItemProperty().addListener(new ChangeListener<String>()
        {
            @Override
            public void changed(ObservableValue<? extends String> observable, String oldValue, String newValue)
            {
                //System.out.println("menu: " + newValue);
                lv_group.setVisible(true);
                lv_music.setTranslateX(0.0);
                //newValue = name of selected item
                if(newValue.compareTo("Music") == 0)
                {
                    ls = dbm.getAllMusic();
                    if(ls.isEmpty())
                    {
                        lv_group.getItems().setAll("<Empty>");
                    }
                    else
                    {
                        List<String> stringList = new ArrayList<String>();
                        for(MediaFile mf : ls)
                        {
                            stringList.add(mf.filename);
                        }
                        lv_music.getItems().setAll(stringList);
                    }
                    lv_group.setVisible(false);
                    lv_music.setTranslateX(-250.0);
                }
                else if(newValue.compareTo("Artist") == 0)
                {
                    ls = dbm.getArtists();
                    if(ls.isEmpty())
                    {
                        lv_group.getItems().setAll("<Empty>");
                    }
                    else
                    {
                        List<String> stringList = new ArrayList<String>();
                        for(MediaFile mf : ls)
                        {
                            stringList.add(mf.filename);
                        }
                        lv_group.getItems().setAll(stringList);
                    }
                    lv_music.getItems().clear();
                    bt_delete.setDisable(true);
                }
                else if(newValue.compareTo("Genre") == 0)
                {
                    ls = dbm.getGenres();
                    if(ls.isEmpty())
                    {
                        lv_group.getItems().setAll("<Empty>");
                    }
                    else
                    {
                        List<String> stringList = new ArrayList<String>();
                        for(MediaFile mf : ls)
                        {
                            stringList.add(mf.filename);
                        }
                        lv_group.getItems().setAll(stringList);
                    }
                    lv_music.getItems().clear();//music should not display when a group is just clicked (only when subgroup is clicked)
                    bt_delete.setDisable(true);
                }
                else if(newValue.compareTo("Playlists") == 0)
                {
                    ls = dbm.getPlaylists();
                    if(ls.isEmpty())
                    {
                        lv_group.getItems().setAll("<Empty>");
                    }
                    else
                    {
                        List<String> stringList = new ArrayList<String>();
                        for(MediaFile mf : ls)
                        {
                            stringList.add(mf.filename);
                        }
                        lv_group.getItems().setAll(stringList);
                    }
                    bt_delete.setDisable(true);
                    lv_music.getItems().clear();
                }
            }
        });

        lv_group.getSelectionModel().selectedItemProperty().addListener(new ChangeListener<String>()
        {
            @Override
            public void changed(ObservableValue<? extends String> observable, String oldValue, String newValue)
            {
                if(newValue!= null)
                {
                    //System.out.println("group: " + newValue);
                    String curGroup = lv_menu.getSelectionModel().getSelectedItem();
                    if(curGroup.compareTo("Artist") == 0)
                    {
                        ls = dbm.getArtist(newValue);
                        List<String> stringList = new ArrayList<String>();
                        for(MediaFile mf : ls)
                        {
                            stringList.add(mf.filename);
                        }
                        lv_music.getItems().setAll(stringList);
                    }
                    else if(curGroup.compareTo("Genre") == 0)
                    {
                        ls = dbm.getGenre(newValue);
                        List<String> stringList = new ArrayList<String>();
                        for(MediaFile mf : ls)
                        {
                            stringList.add(mf.filename);
                        }
                        lv_music.getItems().setAll(stringList);
                    }
                    else if(curGroup.compareTo("Playlists") == 0)
                    {
                        ls = dbm.getPlaylist(newValue);
                        List<String> stringList = new ArrayList<String>();
                        for(MediaFile mf : ls)
                        {
                            stringList.add(mf.filename);
                        }
                        lv_music.getItems().setAll(stringList);
                        bt_delete.setDisable(false);
                    }
                }

            }
        });
        lv_music.getSelectionModel().selectedItemProperty().addListener(new ChangeListener<String>()
        {
            @Override
            public void changed(ObservableValue<? extends String> observable, String oldValue, String newValue)
            {
                //System.out.println("music: " + newValue);
                bt_delete.setDisable(false);
            }
        });

        //play music controls
        volumeSlider.setValue(50);
        volumeSlider.valueProperty().addListener(new InvalidationListener()
        {
            public void invalidated(Observable ov)
            {
                if(mp != null)
                {
                    mp.setVolume(volumeSlider.getValue() / 100.0);
                }
            }
        });

        timeSlider.valueProperty().addListener(new InvalidationListener()
        {
            public void invalidated(Observable ov)
            {
                if(mp != null)
                {
                    // multiply duration by percentage calculated by slider position
                    mp.seek(duration.multiply(timeSlider.getValue() / 100.0));
                }
            }
        });



    }

    public void onScanButton()
    {

        FXMLLoader fxmlLoader = new FXMLLoader(getClass().getResource("scan_ui.fxml"));
        Parent root1 = null;
        try {
            root1 = fxmlLoader.load();
        } catch (IOException e)
        {
            System.out.println("Caught Exception in onScourButton*********************");
            e.printStackTrace();
        }
        Stage stage = new Stage();
        stage.initModality(Modality.APPLICATION_MODAL);
        stage.setScene(new Scene(root1));
        stage.setTitle("Scan for Files");
        stage.showAndWait();

    };
    //remove selected file or playlist from database
    public void onDeleteButton()
    {
        if(lv_menu.getSelectionModel().getSelectedItem().compareTo("Playlists") == 0)
        {
            if(lv_music.getSelectionModel().getSelectedIndex() == -1)//selecting playlist only, no music group
            {
                int selectedIndex = lv_group.getSelectionModel().getSelectedIndex();//playlist
                if(selectedIndex > -1)
                {
                    Alert alert = new Alert(Alert.AlertType.CONFIRMATION);
                    alert.setTitle("Confirmation Dialog");
                    alert.setHeaderText("Delete Playlist: '" + lv_group.getSelectionModel().getSelectedItem() + "'?");
                    alert.setContentText("This operation cannot be undone");

                    Optional<ButtonType> result = alert.showAndWait();
                    if (result.get() == ButtonType.OK)
                    {
                        //find playlist_id by name, delete by playlist_id
                        System.out.println("deleting playlist with id: " + lv_group.getSelectionModel().getSelectedItem());
                    }
                    else
                    {

                    }
                }
            }
            else //selecting music group inside playlist group, delete here removes from playlist
            {
                int musicIndex = lv_music.getSelectionModel().getSelectedIndex();

                if(musicIndex > -1)
                {
                    int selectedIndex = lv_group.getSelectionModel().getSelectedIndex();//playlist

                    Alert alert = new Alert(Alert.AlertType.CONFIRMATION);
                    alert.setTitle("Confirmation Dialog");
                    alert.setHeaderText("Remove Music: '" + ls.get(selectedIndex).filename + "' from Playlist: '" + lv_group.getSelectionModel().getSelectedItem() + "'?");
                    alert.setContentText("(No music files will be deleted)");

                    Optional<ButtonType> result = alert.showAndWait();
                    if (result.get() == ButtonType.OK)
                    {
                        System.out.println("removing music: " + ls.get(selectedIndex).id + " from playlist: " + dbm.getPlaylistId(lv_group.getSelectionModel().getSelectedItem()));
                    }
                    else
                    {

                    }
                }
            }


        }
        else
        {
            int selectedIndex = lv_music.getSelectionModel().getSelectedIndex();
            if(selectedIndex > -1)
            {
                Alert alert = new Alert(Alert.AlertType.CONFIRMATION);
                alert.setTitle("Confirmation Dialog");
                alert.setHeaderText("Delete Music: '" + ls.get(selectedIndex).filename + "'?");
                alert.setContentText("This operation cannot be undone");

                Optional<ButtonType> result = alert.showAndWait();
                if (result.get() == ButtonType.OK)
                {
                    System.out.println("deleting music with id: " + ls.get(selectedIndex).id);
                    dbm.deleteMediaFile(ls.get(selectedIndex).id);
                }
                else
                {

                }
            }
        }
        refresh();
    }
    public void refresh()
    {
        lv_menu.getSelectionModel().select(1);
        lv_menu.getSelectionModel().select(0);
    }
    public void onClearDatabase()
    {

    }

    public void onPlay()
    {
        try
        {
            //m = new MediaPlayer(new Media(ls.get(lv_music.getSelectionModel().getSelectedIndex()).directory));
            int index = lv_music.getSelectionModel().getSelectedIndex();
            if (index > -1)
            {
                MediaFile mf = dbm.getMusicData(ls.get(index).id);
                System.out.println("play: " + mf.directory);
                //File file = new File("C:\\Users\\Turtle\\Desktop\\Musik\\Trance\\Body shine.mp3");
                File file = new File(mf.directory);
                String path = file.toURI().toASCIIString();
                System.out.println("fancy ass url: " + path);
                mp = new MediaPlayer(new Media(path));
                mp.setOnReady(new Runnable()
                {
                    public void run()
                    {
                        duration = mp.getMedia().getDuration();
                        //    playDuration.setText(duration.toString());
                        updateValues();
                        System.out.println("called run()");
                    }
                });

                mp.setVolume(volumeSlider.getValue() / 100);
                mp.play();
                //duration = mp.getMedia().getDuration();
                timeSlider.setValue(0);

                System.out.println("creating new runnable");


                System.out.println("retrieved genre: " + mp.getMedia().getMetadata().get("genre"));
            }

        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
    }
    protected void updateValues()
    {
        Platform.runLater(new Runnable()
        {
            public void run()
            {
                Duration currentTime = mp.getCurrentTime();
                playTime.setText(formatTime(currentTime, duration));
                timeSlider.setDisable(duration.isUnknown());
                if (!timeSlider.isDisabled() && duration.greaterThan(Duration.ZERO) && !timeSlider.isValueChanging())
                {
                    timeSlider.setValue(currentTime.divide(duration).toMillis() * 100.0);
                }
            }
        });
    }
    private static String formatTime(Duration elapsed, Duration duration)
    {
        int intElapsed = (int) Math.floor(elapsed.toSeconds());
        int elapsedHours = intElapsed / (60 * 60);
        if (elapsedHours > 0) {
            intElapsed -= elapsedHours * 60 * 60;
        }
        int elapsedMinutes = intElapsed / 60;
        int elapsedSeconds = intElapsed - elapsedHours * 60 * 60
                - elapsedMinutes * 60;

        if (duration.greaterThan(Duration.ZERO)) {
            int intDuration = (int) Math.floor(duration.toSeconds());
            int durationHours = intDuration / (60 * 60);
            if (durationHours > 0) {
                intDuration -= durationHours * 60 * 60;
            }
            int durationMinutes = intDuration / 60;
            int durationSeconds = intDuration - durationHours * 60 * 60
                    - durationMinutes * 60;
            if (durationHours > 0) {
                return String.format("%d:%02d:%02d/%d:%02d:%02d",
                        elapsedHours, elapsedMinutes, elapsedSeconds,
                        durationHours, durationMinutes, durationSeconds);
            } else {
                return String.format("%02d:%02d/%02d:%02d",
                        elapsedMinutes, elapsedSeconds, durationMinutes,
                        durationSeconds);
            }
        } else {
            if (elapsedHours > 0) {
                return String.format("%d:%02d:%02d", elapsedHours,
                        elapsedMinutes, elapsedSeconds);
            } else {
                return String.format("%02d:%02d", elapsedMinutes,
                        elapsedSeconds);
            }
        }
    }
    public void onStop()
    {
        if(mp != null)
        {
            mp.stop();
        }

    }
    public void onNext()
    {

    }
    public void onPrevious()
    {

    }
}//end Controller.java



