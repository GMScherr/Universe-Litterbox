using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script that controls the gravitational pull of n massive bodies stored in an array. Script is pretty straightforward to use, just add GameObjects to the array and it will calculate all the forces for you.
//Some things to consider :
//-Since every body must attract every body, this script is O(n²). More specifically (n(n-1))/2, still not the most efficient in the world. Careful with how many bodies you add.
//Script fields :
//-massiveBodies : this array with quite the unfortunate name will store the GameObjects that will attract each other. Make sure the transform.position of this GameObject is the true COM of the object.
//-forceArray : a symmetric, hollow adjacency matrix containing the force of gravitational pull between bodies i and j. Symmetric, because the pull is equal for both, and hollow, because a body cannot attract itself.
//-G : The universal gravitational constant. Some big brained science dude came up with it a long time ago so don't ask me what it actually means. We do need it in order to calculate gravity though.
//-G_Multiplier : A necessary evil. Because the mass of a rigidbody is limited to 1+e9 we can't really store the masses of many real life stellar bodies, like the Earth's, which is 15 orders of magnitude larger than
//that. Instead, we can represent the masses as a fraction of what they are while adjusting G accordingly. As such, if we store the mass of the Earth as half of what it is, we must double G. In order to make things
//easier, instead of multiplying G we will change its order of magnitude instead.
//Reminder to implement this later : by dividing the mass by the same multiplier of G you can essentially keep the Force the same while using a lower mass. This is necessary in order to cope with Unity's 10^9 mass
//limit, which means we can't really use kg. However, diminishing the mass while maintaining force increases acceleration in unwanted ways, so the acceleration too will need to be modified, probably divided, by the
//G multiplier. Honestly though, it's 5AM and I can't think straight. It's also December 31st so I'm not gonna deal with this shit right now.
public class nBody_Controller : MonoBehaviour
{
    [SerializeField] private GameObject[] massiveBodies;
    [SerializeField] private float time = 1;
    private double[,] forceArray;
    private double G = 6.67408f * Mathf.Pow(10, -11);
    [SerializeField] private float G_Multiplier = 1;
    private double current_G;
    void Start()
    {
        forceArray = new double[massiveBodies.Length, massiveBodies.Length];
        Time.timeScale = time;
        resetMatrix();
        current_G = G * Mathf.Pow(10, G_Multiplier);
        GameObject p1 = massiveBodies[1];
        Vector3 vec = new Vector3(0.03f,0,0f);
        p1.GetComponent<Rigidbody>().velocity = vec;
    }

    void resetMatrix()
    {
        for (int i = 0; i < massiveBodies.Length; i++)
        {
            for (int j = 0; j < massiveBodies.Length; j++)
            {
                forceArray[i,j] = 0;
            }
        }
    }

    double gravitationalForce(GameObject p1 , GameObject p2)
    {
        float d = Vector3.Distance(p1.transform.position , p2.transform.position);
        if (d == 0)
            return 0;
        double m1 = p1.GetComponent<Rigidbody>().mass;
        double m2 = p2.GetComponent<Rigidbody>().mass;
        double Force = (current_G * m1 * m2)/Mathf.Pow(d,2);
        return Force;
    }

    void nBody()
    {
        for (int i = 0;i < massiveBodies.Length; i++)
        {
            for (int j = 0;j < massiveBodies.Length; j++)
            {
                if(i != j)
                    if (forceArray[i, j] == 0)
                    {
                        forceArray[i, j] = gravitationalForce(massiveBodies[i], massiveBodies[j]);
                        forceArray[j, i] = forceArray[i, j];
                    }
                    Vector3 forceVector = new Vector3();
                    forceVector = massiveBodies[j].transform.position - massiveBodies[i].transform.position;
                    forceVector = forceVector.normalized;
                    massiveBodies[i].GetComponent<Rigidbody>().AddForce(forceVector * (float)forceArray[i, j]);
                    /*Debug.Log(forceArray[i, j]);
                    Debug.Log(i);
                    Debug.Log(j);*/
            }
        }
        resetMatrix();
    }

    // Update is called once per frame
    void Update()
    {
        current_G = G * Mathf.Pow(10, G_Multiplier);
        nBody();
    }
}
