import java.util.ArrayList;
import java.util.List;

public class Node
{
    int _id;//to store hidden and use in retrieval
    String _name;
    String _extension;//.exe, .pdf, etc
    String _category;//(c++, comedy, arts)
    boolean _isComplete;//if I have completed observing this item)
    String _path;//location on disk

    List<String> _tags = new ArrayList<String>();
    String _description;//editable notes I can save on this item
    //boolean _starred;// (checkbox) mark as favorite. replace with 5star rating?


    public Node()
    {
        _name  = "---";
        _extension = "---";
        _category = "---";
        _isComplete = false;
        _path = "---";

        _tags.add("---");//TODO change to parse string
        _description = "---";
    }
    public Node(String filename, String category)
    {
        this();
        _name = filename;
        _category = category;
    }
    public Node(String filename)
    {
        this();
        _name = filename;
        //_category =
    }
    public Node(boolean bool)
    {
        this();
        _isComplete = bool;
    }
    public Node(String name, String ext, String category, boolean isComplete, String path, String tags, String description)
    {
       // this();
        _name  = name;
        _extension = ext;
        _category = category;
        _isComplete = isComplete;
        _path = path;

        _tags.add(tags);//TODO change to parse string
        _description = description;
    }

    public int get_id()
    {
        return _id;
    }

    public String get_name()
    {
        return _name;
    }

    public String get_category()
    {
        return _category;
    }

    public String get_extension() { return _extension; }

    public boolean is_isComplete() { return _isComplete; }

    public String get_path() { return _path; }

    public List<String> get_tags() { return _tags; }

    public String get_description() { return _description; }

    public void set_id(int _id) {
        this._id = _id;
    }

    public void set_name(String _name) {
        this._name = _name;
    }

    public void set_extension(String _extension) {
        this._extension = _extension;
    }

    public void set_category(String _category) {
        this._category = _category;
    }

    public void set_isComplete(boolean _isComplete) {
        this._isComplete = _isComplete;
    }

    public void set_path(String _path) {
        this._path = _path;
    }

    public void set_tags(List<String> _tags) {
        this._tags = _tags;
    }

    public void set_description(String _description) {
        this._description = _description;
    }
}
