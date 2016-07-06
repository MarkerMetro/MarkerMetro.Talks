package com.markermetro.aucklandgdg;

import java.util.Collections;
import java.util.List;

import android.content.Intent;
import android.os.Bundle;
import android.support.v17.leanback.app.BrowseFragment;
import android.support.v17.leanback.widget.ArrayObjectAdapter;
import android.support.v17.leanback.widget.HeaderItem;
import android.support.v17.leanback.widget.ImageCardView;
import android.support.v17.leanback.widget.ListRow;
import android.support.v17.leanback.widget.ListRowPresenter;
import android.support.v17.leanback.widget.OnItemViewClickedListener;
import android.support.v17.leanback.widget.Presenter;
import android.support.v17.leanback.widget.PresenterSelector;
import android.support.v17.leanback.widget.Row;
import android.support.v17.leanback.widget.RowPresenter;
import android.support.v4.app.ActivityOptionsCompat;
import android.util.Log;
import android.view.View;
import android.widget.Toast;

import com.google.gson.Gson;
import com.markermetro.aucklandgdg.cards.CardExampleActivity;
import com.markermetro.aucklandgdg.cards.presenters.CardPresenterSelector;
import com.markermetro.aucklandgdg.details.DetailViewExampleActivity;
import com.markermetro.aucklandgdg.dialog.DialogExampleActivity;
import com.markermetro.aucklandgdg.grid.GridExampleActivity;
import com.markermetro.aucklandgdg.media.VideoExampleActivity;
import com.markermetro.aucklandgdg.models.Card;
import com.markermetro.aucklandgdg.models.CardRow;
import com.markermetro.aucklandgdg.models.Movie;
import com.markermetro.aucklandgdg.page.PageAndListRowActivity;
import com.markermetro.aucklandgdg.settings.SettingsExampleActivity;
import com.markermetro.aucklandgdg.utils.Utils;
import com.markermetro.aucklandgdg.views.ATVHardwareActivity;
import com.markermetro.aucklandgdg.views.AndroidTVActivity;
import com.markermetro.aucklandgdg.views.EndActivity;
import com.markermetro.aucklandgdg.views.WelcomeActivity;
import com.markermetro.aucklandgdg.wizard.WizardExampleActivity;

public class MainFragment extends BrowseFragment {
    private static final String TAG = "MainFragment";

    private static final int NUM_COLS = 15;

    private ArrayObjectAdapter mRowsAdapter;

    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        Log.i(TAG, "onCreate");
        super.onActivityCreated(savedInstanceState);

        setupUIElements();

        loadRows();

        setupEventListeners();
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
    }

    private void loadRows() {
        mRowsAdapter = new ArrayObjectAdapter(new ListRowPresenter());
        String json = Utils.inputStreamToString(getResources().openRawResource(R.raw.launcher_cards));
        CardRow[] rows = new Gson().fromJson(json, CardRow[].class);
        for (CardRow row : rows) {
            mRowsAdapter.add(createCardRow(row));
        }
        setAdapter(mRowsAdapter);
    }

    private ListRow createCardRow(CardRow cardRow) {
        PresenterSelector presenterSelector = new CardPresenterSelector(getActivity());
        ArrayObjectAdapter listRowAdapter = new ArrayObjectAdapter(presenterSelector);
        for (Card card : cardRow.getCards()) {
            listRowAdapter.add(card);
        }
        return new ListRow(new HeaderItem(cardRow.getTitle()), listRowAdapter);
    }

    private void setupUIElements() {
        //setBadgeDrawable(getActivity().getResources().getDrawable(R.drawable.videos_by_google_banner));
        setTitle(getString(R.string.browse_title)); // Badge, when set, takes precedent over title

        setHeadersState(HEADERS_ENABLED);
        setHeadersTransitionOnBackEnabled(true);

        // set fastLane (or headers) background color
        setBrandColor(getResources().getColor(R.color.fastlane_background));
        // set search icon color
        setSearchAffordanceColor(getResources().getColor(R.color.search_color));
    }

    private void setupEventListeners() {
        setOnSearchClickedListener(new View.OnClickListener() {

            @Override
            public void onClick(View view) {
                Toast.makeText(getActivity(), "Implement your own in-app search", Toast.LENGTH_LONG)
                        .show();
            }
        });

        setOnItemViewClickedListener(new ItemViewClickedListener());
    }

    private final class ItemViewClickedListener implements OnItemViewClickedListener {
        @Override
        public void onItemClicked(Presenter.ViewHolder itemViewHolder, Object item,
                                  RowPresenter.ViewHolder rowViewHolder, Row row) {

            Intent intent = null;
            Card card = (Card) item;
            int id = card.getId();
            switch (id) {
                case 0: {
                    intent = new Intent(getActivity().getBaseContext(),
                            CardExampleActivity.class);
                    break;
                }
                case 1:
                    intent = new Intent(getActivity().getBaseContext(),
                            PageAndListRowActivity.class);
                    break;
                case 2: {
                    intent = new Intent(getActivity().getBaseContext(),
                            GridExampleActivity.class);
                    break;
                }
                case 3: {
                    intent = new Intent(getActivity().getBaseContext(),
                            DetailViewExampleActivity.class);
                    break;
                }
                case 4: {
                    intent = new Intent(getActivity().getBaseContext(),
                            VideoExampleActivity.class);
                    break;
                }
                case 6: {
                    // Let's create a new Wizard for a given Movie. The movie can come from any sort
                    // of data source. To simplify this example we decode it from a JSON source
                    // which might be loaded from a server in a real world example.
                    intent = new Intent(getActivity().getBaseContext(),
                            WizardExampleActivity.class);

                    // Prepare extras which contains the Movie and will be passed to the Activity
                    // which is started through the Intent/.
                    Bundle extras = new Bundle();
                    String json = Utils.inputStreamToString(
                            getResources().openRawResource(R.raw.wizard_example));
                    Movie movie = new Gson().fromJson(json, Movie.class);
                    extras.putSerializable("movie", movie);
                    intent.putExtras(extras);

                    // Finally, start the wizard Activity.
                    break;
                }
                case 7: {
                    intent = new Intent(getActivity().getBaseContext(), SettingsExampleActivity.class);
                    startActivity(intent);
                    return;
                }
                case 8: {
                    intent = new Intent(getActivity().getBaseContext(), DialogExampleActivity.class);
                    break;
                }
                case 10: {
                    intent = new Intent(getActivity().getBaseContext(), WelcomeActivity.class);
                    break;
                }
                case 20: {
                    intent = new Intent(getActivity().getBaseContext(), AndroidTVActivity.class);
                    break;
                }
                case 21: {
                    intent = new Intent(getActivity().getBaseContext(), ATVHardwareActivity.class);
                    break;
                }
                case 60: {
                    intent = new Intent(getActivity().getBaseContext(), EndActivity.class);
                    break;
                }
                default:
                    break;
            }
            if (intent != null) {
                Bundle bundle = ActivityOptionsCompat.makeSceneTransitionAnimation(getActivity()).toBundle();
                startActivity(intent, bundle);
            }

        }
    }

}
