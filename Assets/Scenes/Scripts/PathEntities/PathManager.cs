using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Path {
    int id;
    List<PathPiece> pathPieces;
    public Path(int id) {this.id = id;}

    public void AddPiece(PathPiece piece) {
        this.pathPieces.Add(piece);
    }
}
public class PathManager : MonoBehaviour
{
    
    
    List<PathPiece> pathPieces;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
