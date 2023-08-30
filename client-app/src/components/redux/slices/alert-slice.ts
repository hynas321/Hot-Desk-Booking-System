import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export interface Alert {
  text: string;
  visible: boolean;
  type: string;
};

const initialState: Alert = {
  text: "",
  visible: false,
  type: "danger"
};

const gameSettingsSlice = createSlice({
  name: "alert",
  initialState,
  reducers: {
    updatedText(state, action: PayloadAction<string>) {
      state.text = action.payload;
    },
    updatedVisible(state, action: PayloadAction<boolean>) {
      state.visible = action.payload;
    },
    updatedType(state, action: PayloadAction<string>) {
      state.type = action.payload;
    },
    updatedAlert(state, action: PayloadAction<Alert>) {
      state.text = action.payload.text;
      state.visible = action.payload.visible;
      state.type = action.payload.type;
    }
  }
})

export const { updatedText, updatedVisible, updatedType, updatedAlert } = gameSettingsSlice.actions;
export default gameSettingsSlice.reducer;