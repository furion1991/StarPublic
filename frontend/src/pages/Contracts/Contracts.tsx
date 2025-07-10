'use client'

import { useState } from 'react'

import { Button, PageActions } from '@/shared/ui'
import { ContractItemsFillPanel, ContractPrize } from '@/entities/contracts'
import { ItemsForContractList } from '@/widgets/contracts'
import { ContractPrizeActions } from '@/features/contracts'
import { LootItem } from '@/entities/loot'

import { useAuth, useUser } from '@/shared/hooks'

import classes from './Contracts.module.scss'

export const ContractsPage = () => {
  const MAX_ITEMS_IN_CONTRACT_NUMBER = 10

  const { user } = useUser()
  const { isAuth } = useAuth()
  const [itemsIdsInContract, setItemsIdsInContract] = useState<string[]>([])
  const [isContractFinished, setContractFinished] = useState(false)

  const itemsAvailableForContract = user
    ? user.userInventory.itemsUserInventory
        .map(({ id, itemDto }) => {
          const { game, name, image, sellPrice, rarity } = itemDto

          return {
            id,
            game,
            name,
            image,
            sellPrice,
            rarity
          }
        })
        .slice(0, 6)
    : []

  return (
    <>
      <PageActions />

      <div className={classes.contractsPage}>
        <div className={classes.wrapper}>
          <h1>{!isContractFinished ? 'Контракты' : 'Поздравляем!'}</h1>

          {!isContractFinished ? (
            <>
              <div className={classes.fillPanel}>
                <ContractItemsFillPanel
                  itemsInContractNumber={itemsIdsInContract.length}
                  onContractSubmit={() => {
                    if (itemsIdsInContract.length === MAX_ITEMS_IN_CONTRACT_NUMBER) {
                      setContractFinished(true)
                    }
                  }}
                />
              </div>

              {isAuth ? (
                <div className={classes.itemsForContract}>
                  <div className={classes.label}>
                    <p>Доступные для контрактов предметы</p>
                  </div>

                  <div className={classes.itemsList}>
                    <ItemsForContractList
                      itemsIdsInContact={itemsIdsInContract}
                      onItemIdAddToContract={(itemId) => {
                        if (itemsIdsInContract.length < MAX_ITEMS_IN_CONTRACT_NUMBER) {
                          setItemsIdsInContract([...itemsIdsInContract, itemId])
                        }
                      }}
                      onItemIdRemoveFromContract={(itemId) => {
                        setItemsIdsInContract(itemsIdsInContract.filter((id) => itemId !== id))
                      }}
                    />
                  </div>
                </div>
              ) : null}
            </>
          ) : null}

          {isContractFinished ? (
            <>
              <div className={classes.prize}>
                <ContractPrize prizeImg='https://dev.24cases.ru/v1/item/image/583e74e9-9d59-4fa7-b8ac-2e60af7df39b' />
              </div>

              <div className={classes.prizeActions}>
                <ContractPrizeActions
                  prizeSellPrice={900}
                  onContractRetry={() => {
                    setContractFinished(false)
                    setItemsIdsInContract([])
                  }}
                />
              </div>

              <h2>Доступные для контрактов предметы</h2>

              <ul className={classes.availableItemsForContract}>
                {itemsAvailableForContract.map(({ id, name, image, sellPrice, rarity }) => {
                  return (
                    <li key={id}>
                      <LootItem
                        className={classes.lootItem}
                        name={name}
                        image={image}
                        price={sellPrice}
                        rarity={rarity}
                      />

                      <div className={classes.actionBox}>
                        <Button
                          className={classes.addBtn}
                          onClick={() => {
                            setItemsIdsInContract([id])
                            setContractFinished(false)
                          }}
                        >
                          Добавить в контракт
                        </Button>
                      </div>
                    </li>
                  )
                })}
              </ul>
            </>
          ) : null}
        </div>
      </div>
    </>
  )
}
