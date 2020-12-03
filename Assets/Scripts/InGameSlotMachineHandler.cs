using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameSlotMachineHandler : MonoBehaviour
{
    public List<RollingDiceHelper> slot;
    public List<string> rewardedResult = new List<string>();
    private List<string> results = new List<string>();
    public float rewardChancePerc;
    public int numValForEashSlot = 6;

    public Vector3 EndPos;
    public Vector3 StartPos;
    public float speed = 1.0f;
    private Vector3 targetPos;

    public Text txt_result;

    // Start is called before the first frame update
    void Start()
    {
        DoMoveOut();

        int[] slotResultCountingBox = new int[slot.Count];

        if (rewardedResult.Count == 0) {
            Debug.Log("auto generate the awarded result");
            for (int i = 0; i < numValForEashSlot; i++)
            {
                for (int si = 0; si < slot.Count; si++) { slotResultCountingBox[si] = i + 1; }

                string str_r = string.Join(",", slotResultCountingBox);
                rewardedResult.Add(str_r);
            }
        }
        
        //pre-set the reward Result
        results.AddRange(rewardedResult);

        int maxNumResult = (int)( ((float)rewardedResult.Count / rewardChancePerc) - rewardedResult.Count);
        int maxCombination = (int)Mathf.Pow(numValForEashSlot, slot.Count);

        Debug.Log("maxNumResult ? " + maxNumResult);

        //Do generate all results
        List<string> maxResults = new List<string>();
        for (int i = 0; i < maxCombination; i++) {
            for (int si = 0; si < slot.Count; si++) {
                if (si > 0)
                {
                    slotResultCountingBox[si] = ((i / (int)Mathf.Pow(numValForEashSlot, si)) % rewardedResult.Count) + 1;
                }
                else {
                    slotResultCountingBox[si] = (i % rewardedResult.Count) + 1;
                }
            }

            string str_r = string.Join(",", slotResultCountingBox);

            if (!rewardedResult.Contains(str_r))
            {
                Debug.Log("maxResults[" + i+"] --> " + str_r);
                maxResults.Add(str_r);
            }
        }

        //set all result
        while (results.Count < maxNumResult) {
            if (results.Count + maxResults.Count > maxNumResult)
            {
                int lenght = (maxNumResult - results.Count);

                Debug.Log("results while case (1) , Count ? " + results.Count + " , Add lenght ? " + lenght);
                results.InsertRange(lenght, maxResults);
            }
            else {
                results.AddRange(maxResults);
                Debug.Log("results while case (2) , Count ? " + results.Count);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        //// Check if the position of the cube and sphere are approximately equal.
        if (Vector3.Distance(this.transform.position, targetPos) > 0.001f)
        {
            float step = speed * Time.deltaTime; // calculate distance to move
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, step);
        }
    }

    public void DoMoveIn()
    {
        foreach (RollingDiceHelper dice in slot)
        {
            dice.Reset();
        }

        targetPos = EndPos;
    }

    public void DoMoveOut()
    {
        targetPos = StartPos;
    }

    public void SetResult() {

        int randomIdx = Random.Range(0, results.Count -1 );
        string str_result = results[randomIdx];
        Debug.Log("random result ? " + str_result);
        string[] slotResults = str_result.Split(",".ToCharArray());
        if (slotResults.Length == slot.Count)
        {
            foreach (RollingDiceHelper dice in slot)
            {
                int idx = slot.IndexOf(dice);
                int val = 0;
                if (int.TryParse(slotResults[idx],out val))
                {
                    dice.SetResult(val);
                }
                else {
                    Debug.LogWarning("result ["+ idx + "] phase failed !");
                }
            }

            if (txt_result != null) {
                if (rewardedResult.Contains(str_result))
                {
                    txt_result.text = "You Awarded";
                }
                else {
                    txt_result.text = "Get nothing.. Try again";
                }
            }
        }
        else {
            Debug.LogWarning("result count not match your slot count. please check !");
        }
    }


}
