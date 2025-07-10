'use client'

import { notFound, useParams } from 'next/navigation'
import { useEffect, useState } from 'react'

import { BestDroppedItem } from '@/entities/loot'
import { FavoriteCase } from '@/entities/cases'
import { UserHistoryTabs, UserMainInfo, useUserById } from '@/entities/user'
import { InventoryLootList } from '@/widgets/inventory'
import { ContractsHistoryList } from '@/widgets/contracts'
import { UpgradesHistoryList } from '@/widgets/upgrades'

import classes from './UserById.module.scss'

type TabItem = {
  label: string
  value: string
  iconPath: string
  iconActivePath: string
}

export const UserByIdPage = () => {
  const params = useParams<{ id: string }>()
  const userId = params?.id

  const {
    data: user,
    isLoading: isUserLoading,
    error: userLoadError
  } = useUserById({
    id: userId || ''
  })

  const [tab, setTab] = useState('inventory')

  useEffect(() => {
    if (userLoadError) {
      notFound()
    }
  }, [userLoadError])

  const tabs: TabItem[] = [
    {
      label: 'Мой инвентарь',
      value: 'inventory',
      iconPath: '/icons/chest-blue.svg',
      iconActivePath: '/icons/chest-gradient.svg'
    },
    {
      label: 'Апгрейды',
      value: 'upgrades',
      iconPath: '/icons/upgrade-blue.svg',
      iconActivePath: '/icons/upgrade-gradient.svg'
    },
    {
      label: 'Контракты',
      value: 'contracts',
      iconPath: '/icons/medal-blue.svg',
      iconActivePath: '/icons/medal-gradient.svg'
    }
  ]

  if (isUserLoading) return null

  return (
    <div className={classes.wrapper}>
      <div className={classes.userInfoPanel}>
        {user ? (
          <UserMainInfo id={user.id} userName={user.userName} avatar={user.profileImagePath} />
        ) : null}

        <div className={classes.bestDrop}>
          <BestDroppedItem userId={userId || ''} />
        </div>

        <div className={classes.favoriteCase}>
          <FavoriteCase userId={userId || ''} />
        </div>
      </div>

      <div className={classes.userHistory}>
        <div className={classes.userHistoryTopPanel}>
          <UserHistoryTabs activeTab={tab} tabs={tabs} onTabChange={setTab} />
        </div>

        {tab === 'inventory' ? (
          <InventoryLootList
            isLoading={isUserLoading}
            lootItems={user ? user.userInventory.itemsUserInventory : []}
          />
        ) : null}

        {tab === 'upgrades' ? (
          <UpgradesHistoryList upgrades={user ? user.upgradeHistoryRecords : []} />
        ) : null}

        {tab === 'contracts' ? (
          <ContractsHistoryList contracts={user ? user.contractHistoryRecords : []} />
        ) : null}
      </div>
    </div>
  )
}
