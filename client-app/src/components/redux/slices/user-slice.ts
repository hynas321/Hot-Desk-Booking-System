import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export interface User {
  username: string,
  isAdmin: boolean
};

const initialState: User = {
  username: "User",
  isAdmin: false
};

const gameSettingsSlice = createSlice({
  name: "alert",
  initialState,
  reducers: {
    updatedUsername(state, action: PayloadAction<string>) {
      state.username = action.payload;
    },
    updatedIsAdmin(state, action: PayloadAction<boolean>) {
      state.isAdmin = action.payload;
    },
  }
})

export const { updatedUsername, updatedIsAdmin } = gameSettingsSlice.actions;
export default gameSettingsSlice.reducer;