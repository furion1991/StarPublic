export enum InventoryItemStatus {
  NONE = 0,
  FROM_CASE = 1,
  FROM_CONTRACT = 2,
  FROM_UPGRADE = 3,
  WITHDRAWN = 4,
  SOLD = 5,
  USED_ON_CONTRACT = 6,
  USED_ON_UPGRADE = 7
}

type UserBlockStatus = {
  id: string
  isBlocked: boolean
  performedById: string
  reason: string
}

type UserRole = {
  id: string
  name: string
}

type IntentoryLootData = {
  baseCost: number
  game: string
  id: string
  image: string
  isVisible: boolean
  name: string
  rarity: number
  sellPrice: number
  type: number
}

export type InventoryItem = {
  id: string
  itemId: string
  itemRecordState: InventoryItemStatus
  userinventoryid: string
  itemDto: IntentoryLootData
}

type UserInventory = {
  id: string
  itemsUserInventory: InventoryItem[]
  availableInventoryItems: InventoryItem[]
}

type UserStatistics = {
  id: string
  casesBought: number
  ordersPlaced: number
  crashRocketsPlayed: number
  luckBaraban: number
  promocodesUsed: number
}

export type ContractItem = {
  dateOfContract: string
  itemsUsedOnThisContract: IntentoryLootData[]
  resultItem: IntentoryLootData
}

export type UpgradeItem = {
  chance: number
  dateOfUpgrade: string
  isSuccessful: boolean
  itemUsedOnThisUpgrade: IntentoryLootData
  resultItem: IntentoryLootData
}

type DailyBonus = {
  id: string | null
  userId: string | null
  amount: number
  streak: number
  date: string
  isUsedToday: boolean
}

export type User = {
  id: string
  userName: string
  email: string
  profileImagePath: string | null
  phone: string
  currentBalance: number
  dateOfRegistration: string
  isDeleted: boolean
  isSubscribedToTg: boolean
  isSubscribedToVk: boolean

  dailyBonus: DailyBonus
  blockStatus: UserBlockStatus
  userRole: UserRole
  userInventory: UserInventory
  userStatistics: UserStatistics
  contractHistoryRecords: ContractItem[]
  upgradeHistoryRecords: UpgradeItem[]
}

export type UserResponse = {
  message: string
  statusCode: number
  result: User
}
