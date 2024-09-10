using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using JoshH.UI;

public class AIHealthGuage : MonoBehaviour
{
    [Header("REFERENCES")]
    public Slider stunBar;
    public Slider stunBarGhost;
    public Slider healthBar;
    public Slider healthBarGhost;
    public FadeUI rootAlpha;

    public HealthPoints healthPoints;
    private InteractWorldSpaceUI interactiveUI;

    // Start is called before the first frame update
    void Start()
    {
        rootAlpha = GetComponent<FadeUI>();
        healthPoints = GetComponentInParent<HealthPoints>();
        interactiveUI = GetComponentInChildren<InteractWorldSpaceUI>();

        stunBar.maxValue = healthPoints.maxStunPoints;
        stunBarGhost.maxValue = healthPoints.maxStunPoints;

        healthBar.maxValue = healthPoints.maxHealthPoints;
        healthBarGhost.maxValue = healthPoints.maxHealthPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if(healthPoints == null || healthPoints.curHealthPoints <=0)
        {
            rootAlpha.canvasGroup.alpha = 0;
            foreach(UIGradient grad in GetComponentsInChildren<UIGradient>())
            {
                grad.enabled = false;
            }
            return;
        }

        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);

        stunBar.value = Mathf.Lerp(stunBar.value, healthPoints.curStunPoints, 15 * Time.fixedDeltaTime);
        if (Mathf.Abs(stunBar.value - healthPoints.curStunPoints) < 0.1f)
            stunBarGhost.value = Mathf.Lerp(stunBarGhost.value, stunBar.value, 2 * Time.fixedDeltaTime);
        else if(stunBar.value == 0)
            stunBarGhost.value = Mathf.Lerp(stunBarGhost.value, stunBar.value, 2 * Time.fixedDeltaTime);

        healthBar.value = Mathf.Lerp(healthBar.value, healthPoints.curHealthPoints, 15 * Time.fixedDeltaTime);
        if (Mathf.Abs(healthBar.value - healthPoints.curHealthPoints) < 0.1f)
            healthBarGhost.value = Mathf.Lerp(healthBarGhost.value, healthBar.value, 2 * Time.fixedDeltaTime);
        else if (healthBar.value == 0)
            healthBarGhost.value = Mathf.Lerp(healthBarGhost.value, healthBar.value, 2 * Time.fixedDeltaTime);
    }
}
