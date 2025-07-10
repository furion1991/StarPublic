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

type InventoryItem = {
  id: string
  itemId: string
  itemRecordState: number
  userinventoryid: string
  itemDto: IntentoryLootData
}

type UserInventory = {
  id: string
  itemsUserInventory: InventoryItem[]
}

type UserStatistics = {
  id: string
  casesBought: number
  ordersPlaced: number
  crashRocketsPlayed: number
  luckBaraban: number
  promocodesUsed: number
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

  blockStatus: UserBlockStatus
  userRole: UserRole
  userInventory: UserInventory
  userStatistics: UserStatistics
}

export type UserResponse = {
  message: string
  statusCode: number
  result: User
}
