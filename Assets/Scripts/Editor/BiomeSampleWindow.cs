using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BiomeSamplerWindow : EditorWindow {

	/*public static BiomeSampler target;

	public VisualTreeAsset _layout;

	private const int _gridMaxSize = 20;
	private const float _cellSizeX = 50;
	private const float _cellSizeY = 30;
	private const int _startCoordX = 0;
	private const int _startCoordY = 50;
	private const int _biomeListMaxSize = 100;
	private const float _biomeTextureWidth = 50;

	private ListView _biomesListView;
	private ScrollView _biomesGridView;

	private BiomeData[] _biomesList;     // from target
	private int[] _biomeGridId;				// to target

	private int[] _biomesGrid;

	public static void ShowWindow () => GetWindow<BiomeSamplerWindow>(true, "Biome Grid Editor");

	public void CreateGUI () {
		// load layout
		VisualElement root = rootVisualElement;
		_layout.CloneTree(root);

		// pick references to the layout elements
		_biomesListView = root.Q<ListView>("ListBiomes");
		_biomesGridView = root.Q<ScrollView>("Grid");

		CreateGrid();
	}

	private void OnEnable () {
		if (target == null) {
			return;
		}

		if (target.biomesGrid == null || target.biomesGrid.Length == 0) {
			target.biomesGrid = new int[_gridMaxSize * _gridMaxSize];
		}

		target.OnBiomeSamplerListUpdate += UpdateBiomesList;
		_biomesGrid = target.biomesGrid;
		_biomesList = target.biomeDataList;

		UpdateBiomesList();
	}

	private void OnDisable () {
		target.OnBiomeSamplerListUpdate -= UpdateBiomesList;
	}

	private void OnGUI () {
		if (target == null) {
			return;
		}

		if (UpdateBiomesGrid()) {
			ApplyBiomesGrid();
		}
	}

	private bool UpdateBiomesGrid() {
		bool isDirty = false;

		// TODO check if click to add, remove or edit a grid cell
		// TODO update _biomesGrid 
		// TODO show the grid with the buttons

		return isDirty;
	}

	private void ApplyBiomesGrid () {
		Undo.RecordObject(target, "update biomes grid");
		target.biomesGrid = _biomesGrid;
	}

	private void UpdateBiomesList () {
		_biomesList = target.biomeDataList;
	}

	private void CreateGrid () {

		VisualElement currentRow = new VisualElement();

		for (int coordX = 0; coordX < _gridMaxSize; coordX++) {
			for (int coordY = 0; coordY < _gridMaxSize; coordY++) {

				if (coordX == 0) {
					VisualElement row = new VisualElement();
					row.style.flexDirection = FlexDirection.Row;
					_biomesGridView.Add(row);
					currentRow = row;
				}

				VisualElement cellView = new VisualElement();
				cellView.style.width = _cellSizeX;
				cellView.style.height = _cellSizeY;
				cellView.style.borderBottomColor = Color.white;
				cellView.style.borderLeftColor = Color.white;
				cellView.style.borderRightColor = Color.white;
				cellView.style.borderTopColor = Color.white;
				Label label = new Label("FOO");
				cellView.Add(label);
				currentRow.Add(cellView);
			}
		}
	}
	*/
}
