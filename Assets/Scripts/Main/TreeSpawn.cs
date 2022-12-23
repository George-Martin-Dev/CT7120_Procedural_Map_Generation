using UnityEngine;

public class TreeSpawn : MonoBehaviour {
    [SerializeField] private GetPosition GP;

    [SerializeField] private GameObject[] trees;
    private GameObject treePlaceholder;
    [SerializeField] private GameObject treeHolder;

    [SerializeField] private float tree1Probability;
    [SerializeField] private float tree2Probability;
    [SerializeField] private float tree3Probability;
    [SerializeField] private int treeAmount;

    System.Random rng = new System.Random();

    private float choice;

    /// <summary>
    /// Randomly chooses what type of tree to generate. Each tree type has a different probability of being chosen.
    /// </summary>
    public void ChooseTree() {

        for (int i = 0; i < treeAmount; i++) {
            choice = (float)rng.NextDouble();

            if (choice > 0 && choice <= tree1Probability) {
                SpawnTree(trees[0]);
            } else if (choice > tree1Probability && choice <= tree2Probability) {
                SpawnTree(trees[1]);
            } else if (choice > tree2Probability && choice <= tree3Probability) {
                SpawnTree(trees[2]);
            } else if (choice > tree3Probability && choice < 1) {
                SpawnTree(trees[3]);
            }
        }
    }

    /// <summary>
    /// Chooses randomn positions for the trees, keeping in mind certain conditions, one being 
    /// that the trees cannot be withing a certain range of each other.
    /// The tree(s) are then spawned at the random position when the conditions are met.
    /// </summary>
    /// <param name="tree"></param>
    void SpawnTree(GameObject tree) {
        Vector3 startPos = GP.currentChunk.transform.position + new Vector3(15, 0, 15);

        int treeLayer = 1 << 19;

        Vector3 randPos = startPos + new Vector3(rng.Next(-45, 45), 0, rng.Next(-45, 45));
        Collider[] treeColliders;
        treeColliders = Physics.OverlapSphere(randPos, 10, treeLayer);

        while (treeColliders.Length > 0) {
            randPos = startPos + new Vector3(rng.Next(-45, 45), 0, rng.Next(-45, 45));
            treeColliders = Physics.OverlapSphere(randPos, 10, treeLayer);
        }

        treePlaceholder = Instantiate(tree, randPos, Quaternion.identity);
        treePlaceholder.transform.parent = treeHolder.transform;
    }
}
