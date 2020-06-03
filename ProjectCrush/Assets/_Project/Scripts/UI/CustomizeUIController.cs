using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeUIController : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> displayObjects = new List<GameObject>();

	[SerializeField]
	private Button playerDisplayButton, projectileDisplayButton;

	[SerializeField]
	private Slider redSlider, greenSlider, blueSlider;

	[SerializeField]
	private Image redBG, greenBG, blueBG;

	[SerializeField]
	private MeshRenderer playerMaterial, projectileMaterial;

	private float redPlayerChannel, greenPlayerChannel, bluePlayerChannel;
	private float redProjectileChannel, greenProjectileChannel, blueProjectileChannel;
	private bool isPlayer = true;
	
	public void DisplayProjectile()
	{
		displayObjects[0].SetActive(false);
		displayObjects[1].SetActive(true);
		playerDisplayButton.interactable = true;
		projectileDisplayButton.interactable = false;
		isPlayer = false;
		UpdateSliders();
	}

	public void DisplayPlayer()
	{
		displayObjects[0].SetActive(true);
		displayObjects[1].SetActive(false);
		playerDisplayButton.interactable = false;
		projectileDisplayButton.interactable = true;
		isPlayer = true;
		UpdateSliders();
	}

	public void SetRedSliderBGColor()
	{
		if (isPlayer)
		{
			redPlayerChannel = redSlider.value;
			redBG.color = new Color(redPlayerChannel, 0, 0, 1);
			UpdatePlayerMaterial();
		}
		else
		{
			redProjectileChannel = redSlider.value;
			redBG.color = new Color(redProjectileChannel, 0, 0, 1);
			UpdateProjectileMaterial();
		}
	}

	public void SetGreenSliderBGColor()
	{
		if (isPlayer)
		{
			greenPlayerChannel = greenSlider.value;
			greenBG.color = new Color(0, greenPlayerChannel, 0, 1);
			UpdatePlayerMaterial();
		}
		else
		{
			greenProjectileChannel = greenSlider.value;
			greenBG.color = new Color(0, greenProjectileChannel, 0, 1);
			UpdateProjectileMaterial();
		}
	}

	public void SetBlueSliderBGColor()
	{
		if (isPlayer)
		{
			bluePlayerChannel = blueSlider.value;
			blueBG.color = new Color(0, 0, bluePlayerChannel, 1);
			UpdatePlayerMaterial();
		}
		else
		{
			blueProjectileChannel = blueSlider.value;
			blueBG.color = new Color(0, 0, blueProjectileChannel, 1);
			UpdateProjectileMaterial();
		}
	}

	private void UpdatePlayerMaterial()
	{
		playerMaterial.material.color = new Color(redPlayerChannel, greenPlayerChannel, bluePlayerChannel);
		SaveColorChoices();
	}

	private void UpdateProjectileMaterial()
	{
		projectileMaterial.material.color = new Color(redProjectileChannel, greenProjectileChannel, blueProjectileChannel);
		SaveColorChoices();
	}

	private void SaveColorChoices()
	{
		PlayerPrefs.SetFloat("PlayerRedChannel", redPlayerChannel);
		PlayerPrefs.SetFloat("PlayerGreenChannel", greenPlayerChannel);
		PlayerPrefs.SetFloat("PlayerBlueChannel", bluePlayerChannel);
		PlayerPrefs.SetFloat("ProjectileRedChannel", redProjectileChannel);
		PlayerPrefs.SetFloat("ProjectileGreenChannel", greenProjectileChannel);
		PlayerPrefs.SetFloat("ProjectileBlueChannel", blueProjectileChannel);
		PlayerPrefs.Save();
	}

	private void LoadColorChoices()
	{
		redPlayerChannel = PlayerPrefs.GetFloat("PlayerRedChannel", 1);
		greenPlayerChannel = PlayerPrefs.GetFloat("PlayerGreenChannel", 1);
		bluePlayerChannel = PlayerPrefs.GetFloat("PlayerBlueChannel", 1);

		redProjectileChannel = PlayerPrefs.GetFloat("ProjectileRedChannel", 1);
		greenProjectileChannel = PlayerPrefs.GetFloat("ProjectileGreenChannel", 1);
		blueProjectileChannel = PlayerPrefs.GetFloat("ProjectileBlueChannel", 1);
	}

	private void UpdateSliders()
	{
		if (isPlayer)
		{
			redSlider.value = redPlayerChannel;
			greenSlider.value = greenPlayerChannel;
			blueSlider.value = bluePlayerChannel;
		}
		else
		{
			redSlider.value = redProjectileChannel;
			greenSlider.value = greenProjectileChannel;
			blueSlider.value = blueProjectileChannel;
		}
		redBG.color = new Color(redPlayerChannel, 0, 0, 1);
		greenBG.color = new Color(0, greenPlayerChannel, 0, 1);
		blueBG.color = new Color(0, 0, bluePlayerChannel, 1);
	}

	private void Start()
	{
		LoadColorChoices();
		DisplayPlayer();
		UpdateSliders();
	}
}
