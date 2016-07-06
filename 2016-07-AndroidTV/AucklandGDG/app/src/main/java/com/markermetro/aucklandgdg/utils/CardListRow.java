package com.markermetro.aucklandgdg.utils;

import android.support.v17.leanback.widget.HeaderItem;
import android.support.v17.leanback.widget.ListRow;
import android.support.v17.leanback.widget.ObjectAdapter;

import com.markermetro.aucklandgdg.models.CardRow;

/**
 * The {@link CardListRow} allows the {@link ShadowRowPresenterSelector} to access the {@link CardRow}
 * held by the row and determine whether to use a {@link android.support.v17.leanback.widget.Presenter}
 * with or without a shadow.
 */
public class CardListRow extends ListRow {

    private CardRow mCardRow;

    public CardListRow(HeaderItem header, ObjectAdapter adapter, CardRow cardRow) {
        super(header, adapter);
        setCardRow(cardRow);
    }

    public CardRow getCardRow() {
        return mCardRow;
    }

    public void setCardRow(CardRow cardRow) {
        this.mCardRow = cardRow;
    }
}
