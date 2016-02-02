import javafx.fxml.FXML;

import javax.swing.*;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by Turtle on 11/3/2015.
 */
public class DB_Manager
{
    Connection c;
    private static DB_Manager instance = new DB_Manager();//eager instance (we always use this singleton)
    private DB_Manager(){}
    public static DB_Manager getInstance()
    {
        return instance;
    }
    public void init()
    {
        c = null;
        try
        {
            Class.forName("org.sqlite.JDBC");
            c = DriverManager.getConnection("jdbc:sqlite:media_database.db");

        }
        catch(Exception e)
        {
            JOptionPane.showMessageDialog(null, "error in DBM init");
            System.exit(0);
        }
    }
    public void shutdown()
    {
        if(c != null)
        {
            try
            {
                c.close();
            } catch (SQLException e)
            {
                e.printStackTrace();
            }
            c = null;
        }
    }
    //expecting to call from parameterless functions, no danger of SQL injection
    private List<MediaFile> getList(String key, String table, String group)
    {
        List<MediaFile> list = new ArrayList<MediaFile>();
        try
        {
            Statement st = c.createStatement();
            ResultSet rs = st.executeQuery("SELECT " + key + " FROM " + table + " GROUP BY " + group + ";");
            while (rs.next())
            {
                String str = rs.getString(key);
                if(str != null)
                {
                    list.add(new MediaFile(str, null, null, -1));
                }
            }
        }
        catch (SQLException e)
        {
            e.printStackTrace();
        }
        return list;
    }
    List<MediaFile> getAllMusic()
    {
        List<MediaFile> list = new ArrayList<MediaFile>();
        try
        {
            Statement st = c.createStatement();
            ResultSet rs = st.executeQuery("SELECT * FROM music;");
            while (rs.next())
            {
                String str = rs.getString("filename");
                if(str != null)
                {
                    list.add(new MediaFile(str, rs.getString("extension"), null, rs.getInt("music_id")));
                }
            }
        }
        catch (SQLException e)
        {
            e.printStackTrace();
        }
        return list;
    }
    List<MediaFile> getGenres()
    {
        return getList("genre", "music", "genre");
    }
    List<MediaFile> getArtists()
    {
        return getList("artist", "music", "artist");
    }
    List<MediaFile> getPlaylists()
    {
        List<MediaFile> list = new ArrayList<MediaFile>();
        try
        {
            Statement st = c.createStatement();
            ResultSet rs = st.executeQuery("SELECT playlist_name FROM playlists GROUP BY playlist_id;");
            while (rs.next())
            {
                String str = rs.getString("playlist_name");
                if(str != null) list.add(new MediaFile(str, null, null, -1));
            }
        }
        catch (SQLException e)
        {
            e.printStackTrace();
        }
        return list;
    }

    private List<MediaFile> getSpecificList(String group, String name)
    {
        List<MediaFile> artistMusic = null;
        try {
            artistMusic = new ArrayList<MediaFile>();
            PreparedStatement stmt = c.prepareStatement("SELECT * FROM music WHERE lower(" + group + ") = '" + name.toLowerCase() + "';");
            ResultSet rs = stmt.executeQuery();

            // Fetch each row from the result set
            while (rs.next()) {
                String str = rs.getString("filename");
                if(str != null)
                    artistMusic.add(new MediaFile(str, rs.getString("extension"), rs.getString("directory_path"), rs.getInt("music_id")));
            }
        } catch (SQLException e)
        {
            e.printStackTrace();
        }
        return artistMusic;
    }
    List<MediaFile> getGenre(String genreName)
    {
        return getSpecificList("genre", genreName);
    }

    List<MediaFile> getArtist(String artistName)
    {
        return getSpecificList("artist", artistName);
    }

    List<MediaFile> getPlaylist(String playlistName)
    {

        List<MediaFile> playlistMusic = new ArrayList<MediaFile>();
        Statement st = null;
        try
        {
            st = c.createStatement();
            ResultSet rs = st.executeQuery("SELECT playlist_id FROM playlists WHERE lower(playlist_name) = '" + playlistName.toLowerCase() + "';");

            rs.next();
            int id = rs.getInt("playlist_id");//return single playlist id

            rs = st.executeQuery("SELECT music_id FROM playlist_connections WHERE playlist_id = " + id + ";");

            while (rs.next())
            {
                int curID = rs.getInt("music_id");
                Statement st2 = c.createStatement();
                ResultSet rs2 = st2.executeQuery("SELECT * FROM music WHERE music_id = " + curID + ";");
                rs2.next();
                String str = rs2.getString("filename");
                if(str != null)
                    playlistMusic.add(new MediaFile(str, rs2.getString("extension"), rs2.getString("directory_path"), rs2.getInt("music_id")));
            }
            st.close();
        } catch (SQLException e)
        {
            e.printStackTrace();
        }

        return playlistMusic;
    }
    boolean createPlaylist(String playlistName)//TODO implement
    {
        try
        {
            PreparedStatement statement = c.prepareStatement("INSERT INTO playlists (playlist_name) VALUES(?);");
            statement.setString(1, playlistName);
            statement.executeUpdate();
        }
        catch (SQLException e)
        {
            e.printStackTrace();
            return false;
        }
        return true;
    }
    //assumes both playlist and music file have already been added to database and just makes
    //a connection between them
    boolean addToPlaylist(String newPlaylist, int musicID)
    {
        //get playlist id
        int pId = -1;
        int result = 0;
        try
        {
            PreparedStatement st = c.prepareStatement("SELECT playlist_id FROM playlists WHERE playlist_name = ?;");
            st.setString(1, newPlaylist);
            ResultSet rs = st.executeQuery();

            rs.next();
            pId = rs.getInt("playlist_id");


        } catch (SQLException e)
        {
            e.printStackTrace();
        }

        //add playlist_id/music_id combos to playlist_coneections
        try
        {
            PreparedStatement st = c.prepareStatement("INSERT INTO playlist_connections (music_id, playlist_id) VALUES(?,?);");
            st.setInt(1, musicID);
            st.setInt(2, pId);
            result = st.executeUpdate();
        } catch (SQLException e)
        {
            e.printStackTrace();
        }

        if(result == 0) return false; else return true;
    }
    //returns music_id that is automatically generated
    int addMedia(MediaFile media)
    {
        try
        {
            PreparedStatement st = c.prepareStatement(
                    "INSERT INTO music (filename, extension, artist, directory_path, genre) VALUES(?,?,?,?,?);", Statement.RETURN_GENERATED_KEYS);
            st.setString(1, media.getFilename());
            st.setString(2, media.getExt());
            st.setString(3, media.getArtist());
            st.setString(4, media.getDirectory());
            st.setString(5, media.getGenre());
            st.executeUpdate();
            ResultSet rs = st.getGeneratedKeys();
            rs.next();
            return rs.getInt(1);
        }
        catch (SQLException e)
        {
            e.printStackTrace();
            return -1;
        }
    }
    int getPlaylistId(String playlistName)
    {
        int id = -1;
        try
        {
            PreparedStatement st = c.prepareStatement("SELECT playlist_id FROM playlists WHERE LOWER(playlist_name) = ?;");
            st.setString(1, playlistName.toLowerCase());
            ResultSet rs = st.executeQuery();

            if(rs.isBeforeFirst())
            {
                rs.next();
                id = rs.getInt("playlist_id");
            }
            rs.close();
            st.close();
        }
        catch (SQLException e)
        {
            e.printStackTrace();
        }
        return id;
    }
    boolean deletePlaylist(int playlist_id)
    {
        try
        {
            PreparedStatement st = c.prepareStatement("DELETE FROM playlists WHERE playlist_id = " + playlist_id + ";");
            st.executeUpdate();
            st.close();
        }
        catch (SQLException e)
        {
            e.printStackTrace();
            return false;
        }
        breakConnections("playlist_id", playlist_id);
        return true;
    }
    boolean deleteMediaFile(int musicId)
    {
        try
        {
            PreparedStatement st = c.prepareStatement("DELETE FROM music WHERE music_id = " + musicId + ";");
            st.executeUpdate();
            st.close();
        }
        catch (SQLException e)
        {
            e.printStackTrace();
            return false;
        }
        breakConnections("music_id", musicId);
        return true;
    }
    private boolean breakConnections(String key, int musicId)
    {
        try
        {
            PreparedStatement st = c.prepareStatement("DELETE FROM playlist_connections WHERE " + key + " = " + musicId + ";");
            st.executeUpdate();
            st.close();
        }
        catch (SQLException e)
        {
            e.printStackTrace();
            return false;
        }
        return true;
    }

    public MediaFile getMusicData(int musicId)
    {
        MediaFile m = null;
        try
        {
            PreparedStatement stmt = c.prepareStatement("SELECT * FROM music WHERE music_id = " + musicId + ";");
            ResultSet rs = stmt.executeQuery();
            m = new MediaFile(rs.getString("filename"), rs.getString("extension"), rs.getString("directory_path"), musicId);
        }
        catch (SQLException e)
        {
            e.printStackTrace();
        }
        return m;
    }
}